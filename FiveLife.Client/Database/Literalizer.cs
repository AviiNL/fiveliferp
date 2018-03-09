using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Database
{
    internal class Literalizer : ExpressionVisitor
    {
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.DeclaringType.IsDefined(typeof(CompilerGeneratedAttribute), false)
                && node.Expression.NodeType == ExpressionType.Constant)
            {
                object target = ((ConstantExpression)node.Expression).Value, value;
                switch (node.Member.MemberType)
                {
                    case MemberTypes.Property:
                        value = ((PropertyInfo)node.Member).GetValue(target, null);
                        break;
                    case MemberTypes.Field:
                        value = ((FieldInfo)node.Member).GetValue(target);
                        break;
                    default:
                        value = target = null;
                        break;
                }
                if (target != null) return Expression.Constant(value, node.Type);
            }
            return base.VisitMember(node);
        }
    }

}
