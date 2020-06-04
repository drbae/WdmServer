using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tester
{
    public class PatternMatching
    {
        [Fact]
        public static void Run()
        {
            //여러 값을 매칭 ~ tuple 이용
            Console.WriteLine(tupleMatching(0, 0));
            Console.WriteLine(tupleMatching(0, 10));
            Console.WriteLine(tupleMatching(10, 10));
            Console.WriteLine(tupleMatching(1, 1));
            Console.WriteLine();

            //Type 매칭
            Console.WriteLine(typeMatching(new Shape()));
            Console.WriteLine(typeMatching(new Line()));
            Console.WriteLine(typeMatching(new Rectangle()));
            Console.WriteLine(typeMatching(new Circle()));
            Console.WriteLine();

            //Property 매칭
            Console.WriteLine(propertyMatching(new Shape()));
            Console.WriteLine(propertyMatching(new Shape(2, 2)));
            Console.WriteLine();

            //여러 형태 조합 매칭
            Console.WriteLine(testMatching(new Circle(), 9));
            Console.WriteLine(testMatching(new Circle()));
            Console.WriteLine(testMatching(new Circle(2)));
            Console.WriteLine(testMatching(new Circle(3)));
            Console.WriteLine(testMatching(new Shape(4), 4));
            Console.WriteLine(testMatching(new Shape()));
            Console.WriteLine();
        }

        static (string, double) tupleMatching(int score, int level)
        {
            var header = $"{(score, level)} = ";
            double R = (score, level) switch
            {
                (0, 0) => 0,
                var (s, l) when s >= 10 => 0.1,
                var (s, l) when s >= 10 && l >= 10 => 0.5,
                _ => double.NaN
            };
            return (header, R);
        }
        static (string, double) typeMatching(Shape shape)
        {
            return shape switch
            {
                Line _ => (typeof(Line).Name, 0),
                Rectangle r => (r.GetType().Name, r.Width * r.Height),
                Circle c => (c.GetType().Name, Math.PI * c.Radius * c.Radius),
                _ => (shape.GetType().Name, double.NaN)
            };
        }
        static decimal propertyMatching(Shape shape)
        {
            return shape switch
            {
                { V1: 0 } => 0,
                { V1: 1 } => 10,
                { V2: 2 } => 20,
                _ => 30m
            };
        }
        static string testMatching(Shape shape, int inX = 0)
        {
            return shape switch
            {
                Circle { V1: 1 } when inX != 0 => $"Type matching + property matching + when condition: V1={shape.V1}",
                Circle { V1: 1 } => $"Type matching + property matching: V1={shape.V1}",
                Circle { Radius: 2 } => $"Type matching + property matching: Radius={((Circle)shape).Radius} (matched type's property)",
                //Circle c { Radius: 1 } => $"Type matching + property matching : Radius={c.Radius}",//error
                Circle c when c.Radius > 2 => $"Type matching + when condition: Radius={c.Radius}",
                _ when inX > 0 => $"No matching + when condition: {(shape, inX)}",
                _ => "no matching"
            };
        }

        //-------- Classes Under Test --------
        class Shape
        {
            protected List<double> _params;
            public Shape(params double[] args)
            {
                _params = new List<double>();
                if (args.Length != 0) _params.AddRange(args);
                _params.AddRange(new double[] { 1, 2, 3 });
            }
            public double V1 => _params[0];
            public double V2 => _params[1];
        }
        class Line : Shape { }
        class Rectangle : Shape
        {
            public double Width => _params[0];
            public double Height => _params[1];
        }
        class Circle : Shape
        {
            public double Radius => _params[0];
            public Circle() : base() { }
            public Circle(double r) : base(r) { }
        }

    }//class
}
