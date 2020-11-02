using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

public class CodeGen
{
    private static void ImitateStaticClass(CodeTypeDeclaration type)
    {
        @type.TypeAttributes |= TypeAttributes.Sealed;

        @type.Members.Add(new CodeConstructor {
            Attributes = MemberAttributes.Private | MemberAttributes.Final
        });
    }
    
    private static CodeCompileUnit GenerateClassWithConstants(string name, IEnumerable<string> constants)
    {
        var compileUnit = new CodeCompileUnit();
        var @namespace = new CodeNamespace();

        var @class = new CodeTypeDeclaration(name);

        ImitateStaticClass(@class);

        foreach (var constantName in constants)
        {
            var @const = GenerateConstant(constantName);
            @class.Members.Add(@const);
        }

        @namespace.Types.Add(@class);
        compileUnit.Namespaces.Add(@namespace);

        return compileUnit;
    }
    
    private static CodeMemberField GenerateConstant(string name)
    {
        name = name.Replace(" ", "");

        var @const = new CodeMemberField(
            typeof(string),
            name);

        @const.Attributes &= ~MemberAttributes.AccessMask;
        @const.Attributes &= ~MemberAttributes.ScopeMask;
        @const.Attributes |= MemberAttributes.Public;
        @const.Attributes |= MemberAttributes.Const;

        @const.InitExpression = new CodePrimitiveExpression(name);
        return @const;
    }
    
    private static void WriteIntoFile(string fullPath, CodeCompileUnit code)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        using (var stream = new StreamWriter(fullPath, append: false))
        {
            var writer = new IndentedTextWriter(stream);
            using (var codeProvider = new CSharpCodeProvider())
            {
                codeProvider.GenerateCodeFromCompileUnit(code, writer, new CodeGeneratorOptions());
            }
        }
    }
    
    [MenuItem("Window/TheCoreWinUtilities/Generate layers constants")]
    private static void GenerateLayersConstantFile()
    {
        const string path = @"Scripts/Auto/Layers.cs";

        var fullPath = Path.Combine(Application.dataPath, path);
        var className = Path.GetFileNameWithoutExtension(fullPath);

        var code = GenerateClassWithConstants(className, GetAllLayers());
        WriteIntoFile(fullPath, code);

        AssetDatabase.ImportAsset("Assets/" + path, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Window/TheCoreWinUtilities/Generate tag constants")]
    private static void GenerateTagConstantFile()
    {
        const string path = @"Scripts/Auto/Tags.cs";

        var fullPath = Path.Combine(Application.dataPath, path);
        var className = Path.GetFileNameWithoutExtension(fullPath);

        var code = GenerateClassWithConstants(className, GetAllTags());
        WriteIntoFile(fullPath, code);

        AssetDatabase.ImportAsset("Assets/" + path, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();
    }
    
    private static IEnumerable<string> GetAllLayers()
    {
        return InternalEditorUtility.layers;
    }

    private static IEnumerable<string> GetAllTags()
    {
        return InternalEditorUtility.tags;
    }
}
