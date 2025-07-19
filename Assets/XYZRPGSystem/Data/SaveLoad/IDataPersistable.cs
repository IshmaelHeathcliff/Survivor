namespace XYZRPGSystem.Data.SaveLoad
{
    /// <summary>
    /// Classes that implement this interface should have a serialized instance of DataSettings to register through.
    /// </summary>
    public interface IDataPersistable
    {
        string DataTag { get; set; }

        PersistableData SaveData();

        void LoadData(PersistableData persistableData);
    }

    public class PersistableData
    {
    }


    public class PersistableData<T> : PersistableData
    {
        public T Value;

        public PersistableData(T value)
        {
            this.Value = value;
        }
    }


    public class PersistableData<T0, T1> : PersistableData
    {
        public T0 Value0;
        public T1 Value1;

        public PersistableData(T0 value0, T1 value1)
        {
            this.Value0 = value0;
            this.Value1 = value1;
        }
    }


    public class PersistableData<T0, T1, T2> : PersistableData
    {
        public T0 Value0;
        public T1 Value1;
        public T2 Value2;

        public PersistableData(T0 value0, T1 value1, T2 value2)
        {
            this.Value0 = value0;
            this.Value1 = value1;
            this.Value2 = value2;
        }
    }


    public class PersistableData<T0, T1, T2, T3> : PersistableData
    {
        public T0 Value0;
        public T1 Value1;
        public T2 Value2;
        public T3 Value3;

        public PersistableData(T0 value0, T1 value1, T2 value2, T3 value3)
        {
            this.Value0 = value0;
            this.Value1 = value1;
            this.Value2 = value2;
            this.Value3 = value3;
        }
    }


    public class PersistableData<T0, T1, T2, T3, T4> : PersistableData
    {
        public T0 Value0;
        public T1 Value1;
        public T2 Value2;
        public T3 Value3;
        public T4 Value4;

        public PersistableData(T0 value0, T1 value1, T2 value2, T3 value3, T4 value4)
        {
            this.Value0 = value0;
            this.Value1 = value1;
            this.Value2 = value2;
            this.Value3 = value3;
            this.Value4 = value4;
        }
    }
}
