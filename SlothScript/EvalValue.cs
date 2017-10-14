namespace SlothScript
{
    public enum EvalValueType
    {
        INT = 0,
        STRING = 1,
    }
    public class EvalValue
    {
        public static EvalValue ZERO = new EvalValue();
        public static EvalValue TRUE = new EvalValue(1);
        public static EvalValue FALSE = new EvalValue(-1);

        public EvalValue()
        { }

        public EvalValue(int val)
        {
            intVal = val;
            type = EvalValueType.INT;
        }

        public EvalValue(string val)
        {
            strVal = val;
            type = EvalValueType.STRING;
        }

        public EvalValueType type = EvalValueType.INT;
        public int intVal = 0;
        public string strVal = "";
        // 下面这个字段用于标识是否应该直接中断执行
        //public bool hasReturned = false;

        public override string ToString()
        {
            if (type == EvalValueType.INT)
            {
                return intVal.ToString();
            }
            else
            {
                return strVal;
            }
        }

        // + - * / % < > == <= >= && ||

        public static EvalValue operator +(EvalValue lhs, EvalValue rhs)
        {
            if (lhs.type == EvalValueType.INT && rhs.type == EvalValueType.INT)
            {
                return new EvalValue(lhs.intVal + rhs.intVal);
            }
            return new EvalValue(lhs.ToString() + rhs.ToString());
        }

        public static EvalValue operator -(EvalValue lhs, EvalValue rhs)
        {
            if (lhs.type == EvalValueType.STRING || rhs.type == EvalValueType.STRING)
            {
                throw new RunTimeException("字符串不能参与运算" + lhs.ToString() + " - " + rhs.ToString());
            }
            return new EvalValue(lhs.intVal - rhs.intVal);
        }

        public static EvalValue operator *(EvalValue lhs, EvalValue rhs)
        {
            if (lhs.type == EvalValueType.STRING || rhs.type == EvalValueType.STRING)
            {
                throw new RunTimeException("字符串不能参与运算" + lhs.ToString() + " * " + rhs.ToString());
            }
            return new EvalValue(lhs.intVal * rhs.intVal);
        }

        public static EvalValue operator /(EvalValue lhs, EvalValue rhs)
        {
            if (lhs.type == EvalValueType.STRING || rhs.type == EvalValueType.STRING)
            {
                throw new RunTimeException("字符串不能参与运算" + lhs.ToString() + " / " + rhs.ToString());
            }
            return new EvalValue(lhs.intVal / rhs.intVal);
        }

        public static EvalValue operator %(EvalValue lhs, EvalValue rhs)
        {
            if (lhs.type == EvalValueType.STRING || rhs.type == EvalValueType.STRING)
            {
                throw new RunTimeException("字符串不能参与运算" + lhs.ToString() + " % " + rhs.ToString());
            }
            return new EvalValue(lhs.intVal % rhs.intVal);
        }

        public static EvalValue operator <(EvalValue lhs, EvalValue rhs)
        {
            if (lhs.type == EvalValueType.INT && rhs.type == EvalValueType.INT)
            {
                return lhs.intVal < rhs.intVal ? TRUE : FALSE;
            }
            return string.Compare(lhs.ToString(), rhs.ToString()) < 0 ? TRUE : FALSE;
        }

        public static EvalValue operator >(EvalValue lhs, EvalValue rhs)
        {
            if (lhs.type == EvalValueType.INT && rhs.type == EvalValueType.INT)
            {
                return lhs.intVal > rhs.intVal ? TRUE : FALSE;
            }
            return string.Compare(lhs.ToString(), rhs.ToString()) > 0 ? TRUE : FALSE;
        }

        public static EvalValue operator <=(EvalValue lhs, EvalValue rhs)
        {
            if (lhs.type == EvalValueType.INT && rhs.type == EvalValueType.INT)
            {
                return lhs.intVal <= rhs.intVal ? TRUE : FALSE;
            }
            return string.Compare(lhs.ToString(), rhs.ToString()) <= 0 ? TRUE : FALSE;
        }

        public static EvalValue operator >=(EvalValue lhs, EvalValue rhs)
        {
            if (lhs.type == EvalValueType.INT && rhs.type == EvalValueType.INT)
            {
                return lhs.intVal >= rhs.intVal ? TRUE : FALSE;
            }
            return string.Compare(lhs.ToString(), rhs.ToString()) >= 0 ? TRUE : FALSE;
        }
    }
}
