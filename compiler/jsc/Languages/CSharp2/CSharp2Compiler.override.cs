﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using ScriptCoreLib;
using System.Reflection.Emit;
using jsc.Script;
using System.Runtime.InteropServices;

namespace jsc.Languages.CSharp2
{
    partial class CSharp2Compiler
    {
        public override ScriptType GetScriptType()
        {
            return ScriptType.CSharp2;
        }




        public override bool SupportsAbstractMethods
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsBCLTypesAreNative
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsInlineAssigments
        {
            get
            {
                // fixme: seems like OpCodes.ret does not honor if this is set to false
                return true;
            }
        }

        public override System.Reflection.MethodBase ResolveImplementationMethod(Type t, System.Reflection.MethodBase m)
        {
            return MySession.ResolveImplementation(t, m);
        }

        public override System.Reflection.MethodBase ResolveImplementationMethod(Type t, System.Reflection.MethodBase m, string alias)
        {
            return MySession.ResolveMethod(m, t, alias);
        }

        public override void WriteSelf()
        {
            Write("this");
        }


        public override void WriteLocalVariableDefinition(LocalVariableInfo v, MethodBase u)
        {
            WriteIdent();

            WriteKeywordSpace(Keywords._var);
            WriteVariableName(u.DeclaringType, u, v);
            WriteAssignment();
            WriteKeyword(Keywords._default);
            Write("(");
            WriteQualifiedTypeName(u.DeclaringType, v.LocalType);
            Write(")");

            WriteLine(";");
        }

        public override MethodInfo[] GetAllInstanceMethods(Type z)
        {
            if (z == null)
                return null;

            return Enumerable.ToArray(
                from m in base.GetAllInstanceMethods(z)
                let p = new PropertyDetector(m)
                where p.GetProperty == null && p.SetProperty == null
                select m
            );
        }
    }


}
