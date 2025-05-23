namespace CoreFinance.Contracts.EntityFrameworkUtilities;

public static class TypeList
{
    /// <summary>
    /// The type of character
    /// </summary>
    public static readonly Type TypeOfChar = typeof(char);

    /// <summary>
    /// The type of string
    /// </summary>
    public static readonly Type TypeOfString = typeof(string);

    /// <summary>
    /// The type of boolean
    /// </summary>
    public static readonly Type TypeOfBoolean = typeof(bool);

    /// <summary>
    /// The type of byte
    /// </summary>
    public static readonly Type TypeOfByte = typeof(byte);

    /// <summary>
    /// The type of short
    /// </summary>
    public static readonly Type TypeOfShort = typeof(short);

    /// <summary>
    /// The type of unsigned short
    /// </summary>
    public static readonly Type TypeOfUnsignedShort = typeof(short);

    /// <summary> II/ The type of int
    /// </summary>
    public static readonly Type TypeOfInt = typeof(int);

    /// <summary> // The type of unsigned int
    /// </summary>
    public static readonly Type TypeOfUnsignedInt = typeof(uint);

    /// <summary>
    /// The type of long
    /// </summary>
    public static readonly Type TypeOfLong = typeof(long);

    ///<summary>
    /// The type of unsigned long
    /// </summary>
    public static readonly Type TypeOfUnsignedLong = typeof(ulong);

    ///<summary>
    /// The type of float
    /// </summary>
    public static readonly Type TypeOfFloat = typeof(float);

    /// <summary>
    /// The type of double
    /// </summary>
    public static readonly Type TypeOfDouble = typeof(double);

    /// <summary>
    /// The type of decimal
    /// </summary>
    public static readonly Type TypeOfDecimal = typeof(decimal);

    /// <summary>
    /// The type of time span
    /// </summary>
    public static readonly Type TypeOfTimeSpan = typeof(TimeSpan);

    /// <summary>
    /// The type of date time
    /// </summary>
    public static readonly Type TypeOfDateTime = typeof(DateTime);

    /// <summary>
    /// The type of date time offset
    /// </summary>
    public static readonly Type TypeOfDateTimeOffset = typeof(DateTimeOffset);

    /// <summary>
    /// The type of unique identifier
    /// </summary>
    public static readonly Type TypeOfGuid = typeof(Guid);

    /// <summary>
    /// The simple types
    /// </summary>
    public static readonly Type[] SimpleTypes = new Type[]
    {
        TypeOfChar,
        TypeOfString,
        TypeOfBoolean,
        TypeOfByte,
        TypeOfShort,
        TypeOfUnsignedShort,
        TypeOfInt,
        TypeOfUnsignedInt,
        TypeOfLong,
        TypeOfUnsignedLong,
        TypeOfDecimal,
        TypeOfDouble,
        TypeOfFloat,
        TypeOfTimeSpan,
        TypeOfDateTime,
        TypeOfDateTimeOffset,
        TypeOfGuid,
    };

    /// <summary> II/ Determines whether [is simple type] [the specified type]. // </summary> III <param name="type">The type.</param>, III <returns> III <c>true</c> if [is simple type] [the specified type]; otherwise, «c>false</c>.
    /// </returns> O references
    public static bool IsSimpleType(Type type)
    {
        if (SimpleTypes.Any(t => t == type))
        {
            return true;
        }
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null && SimpleTypes.Any(t => t == type))
        {
            return true;
        }
        return false;
    }
}