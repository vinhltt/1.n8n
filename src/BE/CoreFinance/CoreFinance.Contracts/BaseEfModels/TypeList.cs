namespace CoreFinance.Contracts.BaseEfModels;

public static class TypeList
{
    /// <summary>
    /// The type of character
    /// </summary>
    public static readonly Type TypeOfChar = typeof(char);
    /// <summary>
    /// The type of nullable character
    /// </summary>
    public static readonly Type TypeOfNullableChar = typeof(char?);

    /// <summary>
    /// The type of string
    /// </summary>
    public static readonly Type TypeOfString = typeof(string);

    /// <summary>
    /// The type of boolean
    /// </summary>
    public static readonly Type TypeOfBoolean = typeof(bool);

    /// <summary>
    /// The type of nullable boolean
    /// </summary>
    public static readonly Type TypeOfNullableBoolean = typeof(bool?);

    /// <summary>
    /// The type of byte
    /// </summary>
    public static readonly Type TypeOfByte = typeof(byte);

    /// <summary>
    /// The type of nullable byte
    /// </summary>
    public static readonly Type TypeOfNullableByte = typeof(byte?);

    /// <summary>
    /// The type of short
    /// </summary>
    public static readonly Type TypeOfShort = typeof(short);

    /// <summary>
    /// The type of nullable short
    /// </summary>
    public static readonly Type TypeOfNullableShort = typeof(short?);

    /// <summary>
    /// The type of unsigned short
    /// </summary>
    public static readonly Type TypeOfUnsignedShort = typeof(short);

    /// <summary>
    /// The type of nullable unsigned short
    /// </summary>
    public static readonly Type TypeOfNullableUnsignedShort = typeof(short?);

    /// <summary>
    /// The type of int
    /// </summary>
    public static readonly Type TypeOfInt = typeof(int);

    /// <summary>
    /// The type of nullable int
    /// </summary>
    public static readonly Type TypeOfNullableInt = typeof(int?);

    /// <summary> // The type of unsigned int
    /// </summary>
    public static readonly Type TypeOfUnsignedInt = typeof(uint);

    /// <summary>
    /// The type of nullable unsigned int
    /// </summary>
    public static readonly Type TypeOfNullableUnsignedInt = typeof(uint?);

    /// <summary>
    /// The type of long
    /// </summary>
    public static readonly Type TypeOfLong = typeof(long);

    /// <summary>
    /// The type of nullable long
    /// </summary>
    public static readonly Type TypeOfNullableLong = typeof(long?);

    ///<summary>
    /// The type of unsigned long
    /// </summary>
    public static readonly Type TypeOfUnsignedLong = typeof(ulong);

    ///<summary>
    /// The type of nullable unsigned long
    /// </summary>
    public static readonly Type TypeOfNullableUnsignedLong = typeof(ulong?);

    ///<summary>
    /// The type of float
    /// </summary>
    public static readonly Type TypeOfFloat = typeof(float);

    ///<summary>
    /// The type of nullable float
    /// </summary>
    public static readonly Type TypeOfNullableFloat = typeof(float?);

    /// <summary>
    /// The type of double
    /// </summary>
    public static readonly Type TypeOfDouble = typeof(double);

    /// <summary>
    /// The type of nullable double
    /// </summary>
    public static readonly Type TypeOfNullableDouble = typeof(double?);

    /// <summary>
    /// The type of decimal
    /// </summary>
    public static readonly Type TypeOfDecimal = typeof(decimal);

    /// <summary>
    /// The type of nullable decimal
    /// </summary>
    public static readonly Type TypeOfNullableDecimal = typeof(decimal?);

    /// <summary>
    /// The type of time span
    /// </summary>
    public static readonly Type TypeOfTimeSpan = typeof(TimeSpan);

    /// <summary>
    /// The type of nullable time span
    /// </summary>
    public static readonly Type TypeOfNullableTimeSpan = typeof(TimeSpan?);

    /// <summary>
    /// The type of date time
    /// </summary>
    public static readonly Type TypeOfDateTime = typeof(DateTime);

    /// <summary>
    /// The type of nullable date time
    /// </summary>
    public static readonly Type TypeOfNullableDateTime = typeof(DateTime?);

    /// <summary>
    /// The type of date only
    /// </summary>
    public static readonly Type TypeOfDateOnly = typeof(DateOnly);

    /// <summary>
    /// The type of nullable date only
    /// </summary>
    public static readonly Type TypeOfNullableDateOnly = typeof(DateOnly?);

    /// <summary>
    /// The type of time only
    /// </summary>
    public static readonly Type TypeOfTimeOnly = typeof(TimeOnly);

    /// <summary>
    /// The type of nullable time only
    /// </summary>
    public static readonly Type TypeOfNullableTimeOnly = typeof(TimeOnly?);

    /// <summary>
    /// The type of date time offset
    /// </summary>
    public static readonly Type TypeOfDateTimeOffset = typeof(DateTimeOffset);

    /// <summary>
    /// The type of nullable date time offset
    /// </summary>
    public static readonly Type TypeOfNullableDateTimeOffset = typeof(DateTimeOffset?);

    /// <summary>
    /// The type of unique identifier
    /// </summary>
    public static readonly Type TypeOfGuid = typeof(Guid);

    /// <summary>
    /// The type of nullable unique identifier
    /// </summary>
    public static readonly Type TypeOfNullableGuid = typeof(Guid?);

    /// <summary>
    /// The simple types
    /// </summary>
    public static readonly Type[] SimpleTypes =
    {
        TypeOfChar,
        TypeOfNullableChar,
        TypeOfString,
        TypeOfBoolean,
        TypeOfNullableBoolean,
        TypeOfByte,
        TypeOfNullableByte,
        TypeOfShort,
        TypeOfNullableShort,
        TypeOfUnsignedShort,
        TypeOfNullableUnsignedShort,
        TypeOfInt,
        TypeOfNullableInt,
        TypeOfUnsignedInt,
        TypeOfNullableUnsignedInt,
        TypeOfLong,
        TypeOfNullableLong,
        TypeOfUnsignedLong,
        TypeOfNullableUnsignedLong,
        TypeOfDecimal,
        TypeOfNullableDecimal,
        TypeOfDouble,
        TypeOfNullableDouble,
        TypeOfFloat,
        TypeOfNullableFloat,
        TypeOfTimeSpan,
        TypeOfNullableTimeSpan,
        TypeOfDateTime,
        TypeOfNullableDateTime,
        TypeOfDateTimeOffset,
        TypeOfNullableDateTimeOffset,
        TypeOfGuid,
        TypeOfNullableGuid,
        TypeOfDateOnly,
        TypeOfNullableDateOnly,
        TypeOfTimeOnly,
        TypeOfNullableTimeOnly
    };

    /// <summary>
    /// Determines whether [is simple type] [the specified type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    /// <c>true</c> if [is simple type] [the specified type]; otherwise
    /// <c>false</c>.
    /// </returns> 
    public static bool IsSimpleType(Type type)
    {
        if (SimpleTypes.Any(t => t == type))
        {
            return true;
        }
        var underlyingType = Nullable.GetUnderlyingType(type);
        return underlyingType != null && SimpleTypes.Any(t => t == type);
    }

    public static bool IsBoolean(this Type type)
    {
        return type == TypeOfBoolean || type == TypeOfNullableBoolean;
    }

    public static bool IsDateTime(this Type type)
    {
        return type == TypeOfDateTime || type == TypeOfNullableDateTime;
    }

    public static bool IsInt(this Type type)
    {
        return type == TypeOfInt || type == TypeOfNullableInt;
    }

    public static bool IsByte(this Type type)
    {
        return type == TypeOfByte || type == TypeOfNullableByte;
    }

    public static bool IsLong(this Type type)
    {
        return type == TypeOfLong || type == TypeOfNullableLong;
    }

    public static bool IsShort(this Type type)
    {
        return type == TypeOfShort || type == TypeOfNullableShort;
    }

    public static bool IsGuid(this Type type)
    {
        return type == TypeOfGuid || type == TypeOfNullableGuid;
    }

    public static bool IsUnsignedShort(this Type type)
    {
        return type == TypeOfUnsignedShort || type == TypeOfNullableUnsignedShort;
    }

    public static bool IsUnsignedInt(this Type type)
    {
        return type == TypeOfUnsignedInt || type == TypeOfNullableUnsignedInt;
    }

    public static bool IsUnsignedLong(this Type type)
    {
        return type == TypeOfUnsignedLong || type == TypeOfNullableUnsignedLong;
    }

    public static bool IsFloat(this Type type)
    {
        return type == TypeOfFloat || type == TypeOfNullableFloat;
    }

    public static bool IsDouble(this Type type)
    {
        return type == TypeOfDouble || type == TypeOfNullableDouble;
    }

    public static bool IsDecimal(this Type type)
    {
        return type == TypeOfDecimal || type == TypeOfNullableDecimal;
    }

    public static bool IsTimeSpan(this Type type)
    {
        return type == TypeOfTimeSpan || type == TypeOfNullableTimeSpan;
    }

    public static bool IsDateTimeOffset(this Type type)
    {
        return type == TypeOfDateTimeOffset || type == TypeOfNullableDateTimeOffset;
    }

    public static bool IsString(this Type type)
    {
        return type == TypeOfString;
    }
}