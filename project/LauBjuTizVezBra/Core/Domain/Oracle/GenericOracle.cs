namespace Core.Domain.OracleContext;

// Inherit all common properties between the generic_Oracles from BaseOracle

// All of the dynamic data is stored in the generic_Oracles
// Because that is not known at compile time, the user choose which type of oracle to use

public class GenericOracle<T> : BaseOracle // BaseOracle
    where T : class // User, RandomNumbersAI, etc 
{ // Where T is class prevent the use of value types, such as int, double, etc 
    public T Oracle { get; set; } = default!;
    public Type OracleType => Oracle.GetType();
    public GenericOracle(){}
    public GenericOracle(T oracle)
    {
        Oracle = oracle;
    }
}