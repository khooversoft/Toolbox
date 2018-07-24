using System;
using System.Collections.Generic;
using System.Linq;

namespace Khooversoft.Parser
{
    public class AstParser<T> where T : System.Enum
    {
        private readonly HashSet<string> _symbols;
        private readonly AstNode _productionRules;
        private BracketManager<T> _bracketManager;

        public AstParser(AstProductionRules<T> productionRules)
        {
            _productionRules = productionRules?.AstNode ?? throw new ArgumentNullException(nameof(productionRules));
            _bracketManager = new BracketManager<T>(productionRules.Brackets);

            if (_productionRules.OfType<Symbol<T>>().Any(x => string.IsNullOrWhiteSpace(x.GrammarMatch)))
            {
                throw new ArgumentException("Symbol rules don't have grammar match string");
            }

            IEnumerable<IGrammar<T>> grammar = _productionRules
                .BuildGrammars<T>()
                .Concat(productionRules.Grammars ?? Enumerable.Empty<IGrammar<T>>())
                .Concat(_bracketManager.SelectMany(x => x.Grammars));

            var grouping = grammar
                .GroupBy(x => x.GrammarType)
                .Select(x => x.First());

            Tokenizer = new GrammarBuilder<T>(grouping).Build();

            _symbols = new HashSet<string>(Tokenizer.Grammar.Select(x => x.Match));
        }

        public Tokenizer<T> Tokenizer { get; }

        public ParserResult Parse(string rawData)
        {
            rawData = rawData ?? throw new ArgumentNullException(nameof(rawData));

            var context = new ParserContext(Tokenizer.Parse(rawData));

            AstNode result = Match(_productionRules, context);
            if (result != null)
            {
                return new ParserResult(result, context.InputTokens);
            }

            return new ParserResult(context.InputTokens, context.LastGood, context.OutstandingNodes);
        }

        private AstNode Match(AstNode test, ParserContext context)
        {
            var result = new AstNode();
            var stack = new Stack<IAstNode>(test.Reverse());

            while (stack.Count > 0)
            {
                // End of tokens
                if (context.InputTokens.EndOfList)
                {
                    break;
                }

                var testItem = stack.Pop();

                switch (testItem)
                {
                    case Repeat repeat:
                        Process<Repeat>(() => ProcessRepeat(repeat, context));
                        break;

                    case Optional optional:
                        AstNode optionalNode = new AstNode(optional);
                        AstNode optionalResult = TryMatch(optionalNode, context, label: "Optional");
                        if (optionalResult != null)
                        {
                            result += optionalResult;
                            break;
                        }

                        break;

                    case Choice choice:
                        Process<Choice>(() => ProcessChoice(choice, context));
                        break;

                    case AnyOrder anyOrder:
                        Process<AnyOrder>(() => ProcessAnyOrder(anyOrder, context));
                        break;

                    case Body<T> body:
                        Process<Body<T>>(() => ProcessBody(body, context));
                        break;

                    case Skip<T> skip:
                        Process<Skip<T>>(() => ProcessSkip(skip, context));
                        break;

                    case AstNode astNode:
                        Process<AstNode>(() => Match(astNode, context));
                        break;

                    case Expression<T> expression:
                        var tokenValue = context.InputTokens.Next() as TokenValue;
                        if (tokenValue == null)
                        {
                            return null;
                        }

                        // Verify that expression does not equal a symbol
                        if (_symbols.Contains(tokenValue.Value))
                        {
                            return null;
                        }

                        var newExpression = new Expression<T>(expression.TokenType, tokenValue.Value);
                        result += newExpression;
                        break;

                    case Symbol<T> sym:
                        var token = context.InputTokens.Next() as Token<T>;
                        if (token == null)
                        {
                            return null;
                        }

                        if (sym.TokenType.Equals(token.GrammarType))
                        {
                            var newSym = new Symbol<T>(token.GrammarType);
                            result += newSym;
                            break;
                        }

                        AstNode bracketResult = _bracketManager.ProcessBracket(token);
                        if (bracketResult != null)
                        {
                            result += bracketResult;
                            stack.Push(testItem);
                            break;
                        }

                        return null;

                    case Stop stop:
                        stack.Clear();
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                context.LastGood = result.Count > 0 ? result : context.LastGood;
            }

            // Rules didn't finish processing
            if (stack.Count > 0)
            {
                context.OutstandingNodes = stack.ToList();
                return null;
            }

            context.OutstandingNodes = null;
            var returnResult = result.Count > 0 ? result : null;

            context.LastGood = returnResult ?? context.LastGood;

            return returnResult;

            // Local
            bool Process<TNode>(Func<AstNode> processFunc) where TNode : IAstNode
            {
                AstNode testNode = processFunc();
                if (testNode == null)
                {
                    return false;
                }

                result += testNode;
                return true;
            }
        }

        private AstNode TryMatch(AstNode test, ParserContext context, bool commit = true, string label = null)
        {
            context.InputTokens.SaveCursor();

            AstNode result = Match(test, context);
            if (result == null)
            {
                context.InputTokens.RestoreCursor();
                return null;
            }

            if (commit)
            {
                context.InputTokens.AbandonSavedCursor();
            }
            else
            {
                context.InputTokens.RestoreCursor();
            }

            return result;
        }

        private AstNode ProcessRepeat(Repeat repeat, ParserContext context)
        {
            var result = new AstNode();

            while (true)
            {
                AstNode testNode = new AstNode(repeat);
                AstNode testResult = TryMatch(testNode, context);
                if (testResult == null)
                {
                    break;
                }

                result += testResult;
            }

            return result.Count > 0 ? result : null;
        }

        private AstNode ProcessChoice(Choice choice, ParserContext context)
        {
            var result = new AstNode();

            foreach (var test in choice)
            {
                AstNode testNode = new AstNode() + test;
                AstNode testResult = TryMatch(testNode, context);
                if (testResult != null)
                {
                    result += testResult;
                    break;
                }
            }

            return result.Count > 0 ? result : null;
        }

        private AstNode ProcessAnyOrder(AnyOrder anyOrder, ParserContext context)
        {
            var result = new AstNode();
            var queue = new Queue<IAstNode>(anyOrder);
            Queue<IAstNode> missQueue;

            do
            {
                missQueue = new Queue<IAstNode>();
                bool matchOne = false;

                while (queue.Count > 0)
                {
                    IAstNode test = queue.Dequeue();

                    AstNode testNode = new AstNode() + test;
                    AstNode testResult = TryMatch(testNode, context);
                    if (testResult == null)
                    {
                        missQueue.Enqueue(test);
                        continue;
                    }

                    matchOne = true;
                    result += testResult;
                }

                if (!matchOne)
                {
                    return null;
                }

                queue = missQueue;
            }
            while (queue.Count > 0);

            return result.Count > 0 ? result : null;
        }

        private AstNode ProcessBody(Body<T> body, ParserContext context)
        {
            context.InputTokens.SaveCursor();
            var bodyTokenList = new List<IToken>();
            var result = new AstNode();

            Token<T> token = context.InputTokens.Next() as Token<T>;
            if (token == null || !token.GrammarType.Equals(body.StartSymbol.TokenType))
            {
                context.InputTokens.RestoreCursor();
                return null;
            }

            int nestCount = 1;

            while (!context.InputTokens.EndOfList)
            {
                var readToken = context.InputTokens.Next();
                bodyTokenList.Add(readToken);

                token = readToken as Token<T>;
                if (token == null)
                {
                    continue;
                }

                if (token.GrammarType.Equals(body.StartSymbol.TokenType))
                {
                    if (!body.SupportNested)
                    {
                        context.InputTokens.RestoreCursor();
                        return null;
                    }

                    nestCount++;
                    continue;
                }

                if (token.GrammarType.Equals(body.EndSymbol.TokenType))
                {
                    nestCount--;
                    if (nestCount == 0)
                    {
                        context.InputTokens.AbandonSavedCursor();
                        result += new BodyTokens(body, bodyTokenList);
                        return result;
                    }

                    continue;
                }
            }

            return null;
        }

        private AstNode ProcessSkip(Skip<T> skip, ParserContext context)
        {
            var bodyTokenList = new List<IToken>();
            int nestLevel = 0;

            while (!context.InputTokens.EndOfList)
            {

                var token = context.InputTokens.Current as Token<T>;
                if (token != null)
                {
                    if (_bracketManager.IsStartBracket(token))
                    {
                        nestLevel++;
                    }
                    if (_bracketManager.IsEndBracket(token))
                    {
                        nestLevel--;
                        if (nestLevel == 0)
                        {
                            bodyTokenList.Add(context.InputTokens.Next());
                            continue;
                        }
                    }
                }

                if (nestLevel < 0)
                {
                    return null;
                }

                if (nestLevel > 0)
                {
                    bodyTokenList.Add(context.InputTokens.Next());
                    continue;
                }

                foreach (var test in skip)
                {
                    AstNode testNode = new AstNode() + test;
                    AstNode testResult = TryMatch(testNode, context, commit: false);
                    if (testResult != null)
                    {
                        return new AstNode() + new BodyTokens(skip, bodyTokenList);
                    }
                }

                bodyTokenList.Add(context.InputTokens.Next());
            }

            return null;
        }
    }
}
