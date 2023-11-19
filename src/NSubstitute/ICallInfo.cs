using System;

namespace NSubstitute
{
    /// <summary>
    /// Access information on arguments for call.
    /// </summary>
    public interface ICallInfo
    {
        /// <summary>
        /// Gets the nth argument to this call.
        /// </summary>
        /// <param name="index">Index of argument</param>
        /// <returns>The value of the argument at the given index</returns>
        object this[int index] { get; set; }

        /// <summary>
        /// Gets the argument of type `T` passed to this call. This will throw if there are no arguments
        /// of this type, or if there is more than one matching argument.
        /// </summary>
        /// <typeparam name="T">The type of the argument to retrieve</typeparam>
        /// <returns>The argument passed to the call, or throws if there is not exactly one argument of this type</returns>
        T Arg<T>();

        /// <summary>
        /// Gets the argument passed to this call at the specified zero-based position, converted to type `T`.
        /// This will throw if there are no arguments, if the argument is out of range or if it
        /// cannot be converted to the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the argument to retrieve</typeparam>
        /// <param name="position">The zero-based position of the argument to retrieve</param>
        /// <returns>The argument passed to the call, or throws if there is not exactly one argument of this type</returns>
        T ArgAt<T>(int position);

        /// <summary>
        /// Get the arguments passed to this call.
        /// </summary>
        /// <returns>Array of all arguments passed to this call</returns>
        object[] Args();

        /// <summary>
        /// Gets the types of all the arguments passed to this call.
        /// </summary>
        /// <returns>Array of types of all arguments passed to this call</returns>
        Type[] ArgTypes();

        /// <summary>
        /// If we are sure this call returns a value of type <typeparamref name="T"/>, return an
        /// <see cref="ICallInfo{T}"/> that allows us to access the <see cref="ICallInfo{T}.BaseResult"/>.
        ///
        /// This will not be checked by the compiler, so if this method is misused the resulting <see cref="ICallInfo{T}"/>
        /// may throw <see cref="InvalidCastException"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ICallInfo<T> ForCallReturning<T>();
    }

    public interface ICallInfo<T> : ICallInfo
    {
        /// <summary>
        /// Calls the base implementation and attempts to cast the result to <typeparamref name="T"/>.
        /// </summary>
        /// <returns>Result from base (non-substituted) implementation of call</returns>
        /// <exception cref="InvalidCastException" />
        T BaseResult();
    }
}