﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using TinyMapper.Compilers;
using TinyMapper.Compilers.Ast;
using TinyMapper.Compilers.Ast.Statements;
using TinyMapper.Mappers;

namespace TinyMapper.Engines.Builders.Methods
{
    internal sealed class MapMembersMethodBuilder : EmitMethodBuilder
    {
        public MapMembersMethodBuilder(Type sourceType, Type targetType, TypeBuilder typeBuilder)
            : base(sourceType, targetType, typeBuilder)
        {
        }

        protected override void BuildCore()
        {
            CodeGenerator codeGenerator = CreateCodeGenerator(_typeBuilder);
            LocalBuilder localSource = codeGenerator.DeclareLocal(_sourceType);
            LocalBuilder localTarget = codeGenerator.DeclareLocal(_targetType);

            var astComposite = new AstComposite();
            astComposite.Add(LoadMethodArgument(localSource, 1))
                        .Add(LoadMethodArgument(localTarget, 2));

            astComposite.Add(new AstReturn(typeof(object), AstLoadLocal.Load(localTarget)));

            astComposite.Emit(codeGenerator);
        }

        private CodeGenerator CreateCodeGenerator(TypeBuilder typeBuilder)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(ObjectTypeMapper.MapMembersMethodName,
                MethodAttributes.Assembly | MethodAttributes.Virtual, typeof(object), new Type[] { typeof(object), typeof(object) });

            return new CodeGenerator(methodBuilder.GetILGenerator());
        }

        /// <summary>
        /// Loads the method argument.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="argumentIndex">Index of the argument. 0 - This! (start from 1)</param>
        /// <returns><see cref="AstComposite"/></returns>
        private AstComposite LoadMethodArgument(LocalBuilder builder, int argumentIndex)
        {
            var result = new AstComposite();
            result.Add(new AstLocalVariableDeclaration(builder))
                  .Add(new AstStoreLocal(builder, AstLoadArgument.Load(typeof(object), argumentIndex)));
            return result;
        }
    }
}
