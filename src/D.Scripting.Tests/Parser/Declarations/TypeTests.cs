﻿using System.Linq;

using Xunit;

namespace D.Parsing.Tests
{
    using Syntax;

    public class TypeTests : TestBase
    { 
        [Theory]
        [InlineData("Vehicle struct             { name: String     }", TypeFlags.Struct)]
        [InlineData("Vehicle record             { name: String     }", TypeFlags.Record)]
        [InlineData("Vehicle`Crash event        { vehicle: Vehicle }", TypeFlags.Event)]
        [InlineData("Vehicle`Crash event record { vehicle: Vehicle }", TypeFlags.Event | TypeFlags.Record)]
        public void Subtypes(string text, TypeFlags flags)
        {
            var type = Parse<TypeDeclarationSyntax>(text);

            Assert.Equal(flags, type.Flags);
        }

        [Theory]
        [InlineData("SVG struct : Graphic")]
        [InlineData("SVG : Graphic")]         // Optional type
        [InlineData("SVG : Graphic;")]        // Optional semicolon
        public void ExtendsTest(string text)
        {
            var type = Parse<TypeDeclarationSyntax>(text);

            Assert.Equal("SVG", type.Name.ToString());
            Assert.Equal("Graphic", type.BaseType.ToString());
        }

        [Fact]
        public void ReallyLongTerm()
        {
            var type = Parse<TypeDeclarationSyntax>("Type `III `Autoimmune `Polyglandular `Syndrome : Syndrome");

            Assert.Equal("TypeIIIAutoimmunePolyglandularSyndrome", type.Name);
            Assert.Equal("Syndrome", type.BaseType.ToString());
        }

        [Fact]
        public void ShorthandFields()
        {
            var a = Parse<TypeDeclarationSyntax>(@"
Point struct {
  x, y, z: T
}
");
            Assert.Equal(3, a.Members.Length);

            Assert.Equal("x", a.Members[0].Name);
            Assert.Equal("y", a.Members[1].Name);
            Assert.Equal("z", a.Members[2].Name);

            Assert.Equal("T", (a.Members[0] as PropertyDeclarationSyntax).Type);
            Assert.Equal("T", (a.Members[1] as PropertyDeclarationSyntax).Type);
            Assert.Equal("T", (a.Members[2] as PropertyDeclarationSyntax).Type);
        }


        [Fact]
        public void ConstrainedGeneric()
        {
            var a = Parse<TypeDeclarationSyntax>(@"
Size<T:Number> struct {
  width  : Number
  height : Number
}
");
            Assert.Equal(2, a.Members.Length);

            Assert.Equal("T", a.Name.Arguments[0].Name);
            Assert.Equal("width", a.Members[0].Name);
            Assert.Equal("height", a.Members[1].Name);

            Assert.Equal("Number", (a.Members[0] as PropertyDeclarationSyntax).Type);
            Assert.Equal("Number", (a.Members[1] as PropertyDeclarationSyntax).Type);
        }

        [Fact]
        public void Multidefination()
        {
            var type = Parse<CompoundTypeDeclarationSyntax>(@"
A, B, C : D type {
  id : Identity
}");

            Assert.Equal("A", type.Names[0].ToString());
            Assert.Equal("B", type.Names[1].ToString());
            Assert.Equal("C", type.Names[2].ToString());
            Assert.Equal("D", type.BaseType);
        }

        [Fact]
        public void TypeDefination()
        {
            var declaration = Parse<TypeDeclarationSyntax>(@"
Graphic class {
  text : String
  id   : Identity
}");
            Assert.Equal("Graphic",  declaration.Name.ToString());
            Assert.Equal("text",     declaration.Members[0].Name);
            // Assert.Equal("String",   declaration.Members[0].Type.Name);
            Assert.Equal("id",       declaration.Members[1].Name);
            // Assert.Equal("Identity", declaration.Members[1].Type.ToString());

            Assert.Equal(0, declaration.GenericParameters.Length);
        }

        [Fact]
        public void GenericParams()
        {
            var declaration = Parse<TypeDeclarationSyntax>(@"
Point struct <T: Number> : Vector3<T> {
  x: T
  y: T
  z: T
};");
            Assert.Equal("Point", declaration.Name);
            Assert.Equal("Vector3", declaration.BaseType.Name);
            Assert.Equal("T", declaration.BaseType.Arguments[0].Name);

            Assert.Equal(1, declaration.GenericParameters.Length);

            Assert.Equal("T",      declaration.GenericParameters[0].Name);
            Assert.Equal("Number", declaration.GenericParameters[0].Type);

            Assert.Equal(3, declaration.Members.Length);
        }

        /*

        [Fact]
        public void Generic()
        {
            var declaration = Parse<TypeDeclaration>(@"
T record {
  f: ('a, 'a, 'a)
};");

        }
        */

        [Fact]
        public void Q()
        {
            var declaration = Parse<TypeDeclarationSyntax>(@"
T record {
  a: Set<String>
  b: Function<A, B>
  c: * Integer
  d: A | B
  e: (A, B, C)
  f: (A, B, C) -> D
  g: [ physics::Momentum<T> ]
  h: Integer?
  i : [ Collision `Course? ]
};");


            var members = declaration.Members.Cast<PropertyDeclarationSyntax>().ToArray();

            Assert.Equal("T",                               declaration.Name);

            Assert.Equal("Set",                             members[0].Type.Name);
            Assert.Equal("String",                          members[0].Type.Arguments[0].Name);

            Assert.Equal("Function",                        members[1].Type.Name);
            Assert.Equal("A",                               members[1].Type.Arguments[0].Name);
            Assert.Equal("B",                               members[1].Type.Arguments[1].Name);

            Assert.Equal("Channel",                         members[2].Type.Name);
            Assert.Equal("Integer",                         members[2].Type.Arguments[0].Name);

            Assert.Equal("Variant<A,B>",                    members[3].Type.ToString());
            Assert.Equal("Tuple<A,B,C>",                    members[4].Type.ToString());
            Assert.Equal("Function<A,B,C,D>",               members[5].Type.ToString());
            Assert.Equal("List<physics::Momentum<T>>",      members[6].Type.ToString());
            Assert.Equal("Optional<Integer>",               members[7].Type.ToString());
            Assert.Equal("List<Optional<CollisionCourse>>", members[8].Type.ToString());
        }

        [Fact]
        public void Complicated()
        {
            var type = Parse<TypeDeclarationSyntax>(@"
Account record {
   mutable balance :   Decimal
   owner           :   Entity
   provider        :   Organization
   transactions    : [ Transaction ]
   currencyCode    :   List<Character>
};");

            var members = type.Members;

            Assert.Equal("Account", type.Name.ToString());
            Assert.True(type.IsRecord);
            Assert.True((members[0] as PropertyDeclarationSyntax).Flags.HasFlag(ObjectFlags.Mutable));
            Assert.Equal("balance", members[0].Name);
            Assert.Equal("Decimal", (members[0] as PropertyDeclarationSyntax).Type.ToString());

            Assert.Equal("owner",   members[1].Name);
            Assert.Equal("Entity", (members[1] as PropertyDeclarationSyntax).Type);

            Assert.Equal("Organization", (members[2] as PropertyDeclarationSyntax).Type);

            Assert.Equal("List",        (members[3] as PropertyDeclarationSyntax).Type.Name);
            Assert.Equal("Transaction", (members[3] as PropertyDeclarationSyntax).Type.Arguments[0].Name);

            Assert.Equal("List",        (members[4] as PropertyDeclarationSyntax).Type.Name);
            Assert.Equal("Character",   (members[4] as PropertyDeclarationSyntax).Type.Arguments[0].Name);
        }
    }
}