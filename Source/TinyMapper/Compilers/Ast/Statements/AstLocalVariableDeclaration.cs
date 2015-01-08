﻿using System;
using System.Reflection.Emit;
using TinyMapper.Nelibur.Sword.DataStructures;
using TinyMapper.Nelibur.Sword.Extensions;

namespace TinyMapper.Compilers.Ast.Statements
{
    internal sealed class AstLocalVariableDeclaration : IAstType
    {
        private readonly Option<LocalBuilder> _localBuilder;

        public AstLocalVariableDeclaration(LocalBuilder localBuilder)
        {
            _localBuilder = localBuilder.ToOption();
            ObjectType = localBuilder.LocalType;
        }

        public Type ObjectType { get; private set; }

        public void Emit(CodeGenerator generator)
        {
            _localBuilder.Where(x => x.LocalType.IsValueType)
                         .Do(x => generator.Emit(OpCodes.Ldloca, x.LocalIndex))
                         .Do(x => generator.Emit(OpCodes.Initobj, x.LocalType));
        }
    }
}
