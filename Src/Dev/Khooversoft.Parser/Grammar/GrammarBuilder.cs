using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Parser
{
    public class GrammarBuilder<T> : IEnumerable<IGrammar<T>> where T : System.Enum
    {
        private readonly StringComparison _stringComparison;

        public GrammarBuilder(StringComparison stringComparison = StringComparison.Ordinal)
        {
            _stringComparison = stringComparison;
        }

        public GrammarBuilder(IEnumerable<IGrammar<T>> grammars, StringComparison stringComparison = StringComparison.Ordinal)
            : this(stringComparison)
        {
            foreach(var item in grammars)
            {
                Grammar[item.GrammarType] = item;
            }
        }

        public Dictionary<T, IGrammar<T>> Grammar { get; } = new Dictionary<T, IGrammar<T>>();

        public void Add(IGrammar<T> grammar)
        {
            Grammar[grammar.GrammarType] = grammar;
        }

        public Tokenizer<T> Build()
        {
            return new Tokenizer<T>(Grammar.Values, _stringComparison);
        }

        public IEnumerator<IGrammar<T>> GetEnumerator()
        {
            return Grammar.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Grammar.Values.GetEnumerator();
        }
    }
}
