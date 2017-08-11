module ExtractDocs =
    open System
    open System.IO
    open System.Text.RegularExpressions

    let LiquidTagRegex = @"(?:\<!--\s*)?\{%\s*(?<tag>\w+).*\%}(?:\s*--\>)?" + // Tag start, optionally surrounded by HTML comments
                                                                              //     e.g. "<!-- {% code %} -->"
                         @"(?<contents>(?s:.*?))" +                           // Tag contents
                         @"(?:\<!--\s*)?\{%\s*end\1\s*%\}(?:\s*--\>)?"        // Tag end w/optional comments. e.g. "<!-- {% endcode %} -->"
    let TypeOrTestDeclRegex = @"(\[Test\]|(public |private |protected )?(class |interface )\w+\s*\{)"

    type LiquidTag = LiquidTag of name : string * contents : string
    type CodeBlock = Declaration of string | Snippet of string

    let tags s : LiquidTag seq =
        Regex.Matches(s, LiquidTagRegex)
        |> Seq.cast<Match>
        |> Seq.map (fun m -> LiquidTag (m.Groups.[1].Value, m.Groups.[2].Value))

    let toCodeBlock (LiquidTag (name, c)) =
        let isTypeOrTestDecl s = Regex.IsMatch(s, TypeOrTestDeclRegex)
        match name with
        | "examplecode"  -> if isTypeOrTestDecl c then Some (Declaration c) else Some (Snippet c)
        | "requiredcode" -> Some (Declaration c)
        | _              -> None

    let toCodeBlocks : string -> CodeBlock seq =
        Seq.choose toCodeBlock << tags

    let toFixture name content = 
        let escapeName (s:string) = Regex.Replace(s, "\W", "_")
        sprintf """
    using System;
    using NUnit.Framework;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel;
    using NSubstitute.Extensions;

    namespace NSubstitute.Samples {
        public class Tests_%s {
            %s
        }
    }""" (escapeName name) content

    let toTest = sprintf """[Test] public void Test_%d() {
    %s
    }"""

    let appendCodeBlock (code, testNum) (cb:CodeBlock) =
        match cb with
        | Declaration d -> (code + Environment.NewLine + d, testNum)
        | Snippet s -> 
            let test = toTest testNum s
            (code + Environment.NewLine + test, testNum + 1)

    let strToFixture fixtureName s : string =
        s
        |> toCodeBlocks
        |> Seq.fold appendCodeBlock ("", 0)
        |> fst
        |> toFixture fixtureName