using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class Tokenizer<T> where T : System.Enum
    {
        private readonly string[] _delimiters;
        private readonly Dictionary<string, Grammar<T>> _matchGrammar;
        private readonly StringComparison _stringComparison;

        public Tokenizer(IEnumerable<IGrammar<T>> grammar, StringComparison stringComparison = StringComparison.Ordinal)
        {
            _stringComparison = stringComparison;

            Grammar = grammar.ToList();

            var grouping = Grammar
                .OfType<Grammar<T>>()
                .GroupBy(x => x.Match)
                .Where(x => x.Count() > 1);

            if(grouping.Any())
            {
                throw new ArgumentException($"Cannot add duplicate 'match' symbols, {string.Join(", ", grouping.Select(x => x.Key))}");
            }

            _matchGrammar = Grammar
                .OfType<Grammar<T>>()
                .ToDictionary(x => x.Match, stringComparison == StringComparison.Ordinal ? null : StringComparer.OrdinalIgnoreCase);

            _delimiters = _matchGrammar.Values
                .Select(x => x.Match)
                .ToArray();
        }

        public IReadOnlyList<IGrammar<T>> Grammar { get; }

        public IEnumerable<IToken> Parse(string line)
        {
            List<IToken> tokens = new List<IToken>();
            Stack<string> stack = new Stack<string>(line.Tokenize(_delimiters, _stringComparison).Reverse());

            while (stack.Count > 0)
            {
                string rawToken = stack.Pop();
                if( string.IsNullOrWhiteSpace(rawToken) )
                {
                    continue;
                }

                if (_matchGrammar.TryGetValue(rawToken, out Grammar<T> value))
                {
                    tokens.Add(value.CreateToken());
                    continue;
                }

                tokens.Add(new TokenValue(rawToken));
            }

            return tokens;
        }
    }
}
