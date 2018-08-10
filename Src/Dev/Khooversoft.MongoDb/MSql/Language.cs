//using Khooversoft.Toolbox.Parser;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Khooversoft.MongoDb.MSql
//{
//    public static class Language
//    {
//        public static Grammar<MongoTokenType> Space { get; } = new Grammar<MongoTokenType>(MongoTokenType.Space, " ");

//        public static Symbol<MongoTokenType> SymSelect { get; } = new Symbol<MongoTokenType>(MongoTokenType.Select, "SELECT");
//        public static Symbol<MongoTokenType> SymDistinct { get; } = new Symbol<MongoTokenType>(MongoTokenType.Distinct, "DISTINCT");
//        public static Symbol<MongoTokenType> SymFrom { get; } = new Symbol<MongoTokenType>(MongoTokenType.From, "FROM");
//        public static Symbol<MongoTokenType> SymLeft { get; } = new Symbol<MongoTokenType>(MongoTokenType.Left, "LEFT");
//        public static Symbol<MongoTokenType> SymJoin { get; } = new Symbol<MongoTokenType>(MongoTokenType.Join, "JOIN");
//        public static Symbol<MongoTokenType> SymOuter { get; } = new Symbol<MongoTokenType>(MongoTokenType.Outer, "OUTER");
//        public static Symbol<MongoTokenType> SymInner { get; } = new Symbol<MongoTokenType>(MongoTokenType.Inner, "INNER");
//        public static Symbol<MongoTokenType> SymAs { get; } = new Symbol<MongoTokenType>(MongoTokenType.As, "AS");
//        public static Symbol<MongoTokenType> SymOn { get; } = new Symbol<MongoTokenType>(MongoTokenType.On, "ON");
//        public static Symbol<MongoTokenType> SymSum { get; } = new Symbol<MongoTokenType>(MongoTokenType.On, "SUM");
//        public static Symbol<MongoTokenType> SymAnd { get; } = new Symbol<MongoTokenType>(MongoTokenType.On, "AND");

//        public static Symbol<MongoTokenType> SymEqual { get; } = new Symbol<MongoTokenType>(MongoTokenType.Equal, "=");
//        public static Symbol<MongoTokenType> SymComma { get; } = new Symbol<MongoTokenType>(MongoTokenType.Comma, ",");
//        public static Symbol<MongoTokenType> SymLessThan { get; } = new Symbol<MongoTokenType>(MongoTokenType.LessThan, "<");
//        public static Symbol<MongoTokenType> SymGreaterThan { get; } = new Symbol<MongoTokenType>(MongoTokenType.GreaterThan, ">");
//        public static Symbol<MongoTokenType> SymSemiColon { get; } = new Symbol<MongoTokenType>(MongoTokenType.SemiColon, ";");
//        public static Symbol<MongoTokenType> SymLeftParen { get; } = new Symbol<MongoTokenType>(MongoTokenType.LeftParen, "(");
//        public static Symbol<MongoTokenType> SymRightParen { get; } = new Symbol<MongoTokenType>(MongoTokenType.RightParen, ")");

//        public static Expression<MongoTokenType> TableName { get; } = new Expression<MongoTokenType>(MongoTokenType.TableName);
//        public static Expression<MongoTokenType> ColumnName { get; } = new Expression<MongoTokenType>(MongoTokenType.ColumnName);
//        public static Expression<MongoTokenType> Alias { get; } = new Expression<MongoTokenType>(MongoTokenType.Alias);
//    }
//}
