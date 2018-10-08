using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Khooversoft.Toolbox.Test.Tools
{
    [Trait("Category", "Toolbox")]
    public class CursorListTests
    {
        [Fact]
        public void SimpleAddAndReadTest()
        {
            const int size = 100;
            var cl = new CursorList<string>();
            var saveList = new List<string>();

            foreach (var index in Enumerable.Range(0, size))
            {
                string msg = $"Index_{index}";
                cl.Add(msg);
                saveList.Add(msg);
            }

            cl.Count.Should().Be(size);
            saveList.Count.Should().Be(size);

            cl.Zip(saveList, (o, i) => new { o, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();

            foreach (var index in Enumerable.Range(0, size))
            {
                cl[index].Should().Be(saveList[index]);
            }

            cl.Clear();
            cl.Count.Should().Be(0);
            cl.EndOfList.Should().BeTrue();
        }

        [Fact]
        public void UseCurorTest()
        {
            const int size = 100;
            var cl = new CursorList<string>();

            foreach (var index in Enumerable.Range(0, size))
            {
                string msg = $"Index_{index}";
                cl.Add(msg);
            }

            cl.Count.Should().Be(size);

            var testList = new List<string>();
            while (!cl.EndOfList)
            {
                testList.Add(cl.Next());
            }

            testList.Count.Should().Be(size);

            cl.Zip(testList, (o, i) => new { o, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }

        [Fact]
        public void SetCursorTest()
        {
            const int size = 100;
            var cl = new CursorList<string>();

            foreach (var index in Enumerable.Range(0, size))
            {
                string msg = $"Index_{index}";
                cl.Add(msg);
            }

            cl.Count.Should().Be(size);

            var testList = new List<string>(Enumerable.Range(0, size).Select(x => ""));
            foreach (var index in Enumerable.Range(0, size))
            {
                testList[index] = cl.Current;
                cl.Cursor++;
            }

            bool test = false;
            try
            {
                cl.Cursor++;
                cl.Next();
                test = true;
            }
            catch (InvalidOperationException)
            {
                test = false;
            }
            test.Should().BeFalse();

            testList.Count.Should().Be(size);

            cl.Zip(testList, (o, i) => new { o, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }

        [Fact]
        public void CursorPushPopTest()
        {
            const int size = 100;
            var cl = new CursorList<string>();
            var rnd = new Random();

            foreach (var index in Enumerable.Range(0, size))
            {
                string msg = $"Index_{index}";
                cl.Add(msg);
            }

            cl.Count.Should().Be(size);

            int rndIndex = rnd.Next(0, size - 10);
            foreach (var index in Enumerable.Range(0, rndIndex))
            {
                cl.Next();
            }

            int saveCursor = cl.Cursor;
            string saveValue = cl.Current;
            cl.SaveCursor();

            foreach (var index in Enumerable.Range(0, 5))
            {
                cl.Next();
            }

            saveValue.Should().NotBe(cl.Current);
            (saveCursor + 5).Should().Be(cl.Cursor);

            cl.RestoreCursor();
            saveCursor.Should().Be(cl.Cursor);
            saveValue.Should().Be(cl.Current);
        }
    }
}
