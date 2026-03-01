// MIT License
// Copyright (c) 2026 Single Finite
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace SingleFinite.Essentials;

/// <summary>
/// Extension functions for the Func class.
/// </summary>
public static class FuncExtensions
{
    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <param name="func">The func to invoke.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke(
        this Func<Task>? func
    ) =>
        func?.Invoke() ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1>(
        this Func<T1, Task>? func,
        T1 t1
    ) =>
        func?.Invoke(t1) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2>(
        this Func<T1, T2, Task>? func,
        T1 t1, T2 t2
    ) =>
        func?.Invoke(t1, t2) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3>(
        this Func<T1, T2, T3, Task>? func,
        T1 t1, T2 t2, T3 t3
    ) =>
        func?.Invoke(t1, t2, t3) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <typeparam name="T4">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <param name="t4">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3, T4>(
        this Func<T1, T2, T3, T4, Task>? func,
        T1 t1, T2 t2, T3 t3, T4 t4
    ) =>
        func?.Invoke(t1, t2, t3, t4) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <typeparam name="T4">An argument type.</typeparam>
    /// <typeparam name="T5">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <param name="t4">An argument to pass to the func.</param>
    /// <param name="t5">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3, T4, T5>(
        this Func<T1, T2, T3, T4, T5, Task>? func,
        T1 t1, T2 t2, T3 t3, T4 t4, T5 t5
    ) =>
        func?.Invoke(t1, t2, t3, t4, t5) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <typeparam name="T4">An argument type.</typeparam>
    /// <typeparam name="T5">An argument type.</typeparam>
    /// <typeparam name="T6">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <param name="t4">An argument to pass to the func.</param>
    /// <param name="t5">An argument to pass to the func.</param>
    /// <param name="t6">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3, T4, T5, T6>(
        this Func<T1, T2, T3, T4, T5, T6, Task>? func,
        T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6
    ) =>
        func?.Invoke(t1, t2, t3, t4, t5, t6) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <typeparam name="T4">An argument type.</typeparam>
    /// <typeparam name="T5">An argument type.</typeparam>
    /// <typeparam name="T6">An argument type.</typeparam>
    /// <typeparam name="T7">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <param name="t4">An argument to pass to the func.</param>
    /// <param name="t5">An argument to pass to the func.</param>
    /// <param name="t6">An argument to pass to the func.</param>
    /// <param name="t7">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3, T4, T5, T6, T7>(
        this Func<T1, T2, T3, T4, T5, T6, T7, Task>? func,
        T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7
    ) =>
        func?.Invoke(t1, t2, t3, t4, t5, t6, t7) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <typeparam name="T4">An argument type.</typeparam>
    /// <typeparam name="T5">An argument type.</typeparam>
    /// <typeparam name="T6">An argument type.</typeparam>
    /// <typeparam name="T7">An argument type.</typeparam>
    /// <typeparam name="T8">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <param name="t4">An argument to pass to the func.</param>
    /// <param name="t5">An argument to pass to the func.</param>
    /// <param name="t6">An argument to pass to the func.</param>
    /// <param name="t7">An argument to pass to the func.</param>
    /// <param name="t8">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, Task>? func,
        T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8
    ) =>
        func?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <typeparam name="T4">An argument type.</typeparam>
    /// <typeparam name="T5">An argument type.</typeparam>
    /// <typeparam name="T6">An argument type.</typeparam>
    /// <typeparam name="T7">An argument type.</typeparam>
    /// <typeparam name="T8">An argument type.</typeparam>
    /// <typeparam name="T9">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <param name="t4">An argument to pass to the func.</param>
    /// <param name="t5">An argument to pass to the func.</param>
    /// <param name="t6">An argument to pass to the func.</param>
    /// <param name="t7">An argument to pass to the func.</param>
    /// <param name="t8">An argument to pass to the func.</param>
    /// <param name="t9">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task>? func,
        T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9
    ) =>
        func?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <typeparam name="T4">An argument type.</typeparam>
    /// <typeparam name="T5">An argument type.</typeparam>
    /// <typeparam name="T6">An argument type.</typeparam>
    /// <typeparam name="T7">An argument type.</typeparam>
    /// <typeparam name="T8">An argument type.</typeparam>
    /// <typeparam name="T9">An argument type.</typeparam>
    /// <typeparam name="T10">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <param name="t4">An argument to pass to the func.</param>
    /// <param name="t5">An argument to pass to the func.</param>
    /// <param name="t6">An argument to pass to the func.</param>
    /// <param name="t7">An argument to pass to the func.</param>
    /// <param name="t8">An argument to pass to the func.</param>
    /// <param name="t9">An argument to pass to the func.</param>
    /// <param name="t10">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task>? func,
        T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10
    ) =>
        func?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <typeparam name="T4">An argument type.</typeparam>
    /// <typeparam name="T5">An argument type.</typeparam>
    /// <typeparam name="T6">An argument type.</typeparam>
    /// <typeparam name="T7">An argument type.</typeparam>
    /// <typeparam name="T8">An argument type.</typeparam>
    /// <typeparam name="T9">An argument type.</typeparam>
    /// <typeparam name="T10">An argument type.</typeparam>
    /// <typeparam name="T11">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <param name="t4">An argument to pass to the func.</param>
    /// <param name="t5">An argument to pass to the func.</param>
    /// <param name="t6">An argument to pass to the func.</param>
    /// <param name="t7">An argument to pass to the func.</param>
    /// <param name="t8">An argument to pass to the func.</param>
    /// <param name="t9">An argument to pass to the func.</param>
    /// <param name="t10">An argument to pass to the func.</param>
    /// <param name="t11">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task>? func,
        T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11
    ) =>
        func?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <typeparam name="T4">An argument type.</typeparam>
    /// <typeparam name="T5">An argument type.</typeparam>
    /// <typeparam name="T6">An argument type.</typeparam>
    /// <typeparam name="T7">An argument type.</typeparam>
    /// <typeparam name="T8">An argument type.</typeparam>
    /// <typeparam name="T9">An argument type.</typeparam>
    /// <typeparam name="T10">An argument type.</typeparam>
    /// <typeparam name="T11">An argument type.</typeparam>
    /// <typeparam name="T12">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <param name="t4">An argument to pass to the func.</param>
    /// <param name="t5">An argument to pass to the func.</param>
    /// <param name="t6">An argument to pass to the func.</param>
    /// <param name="t7">An argument to pass to the func.</param>
    /// <param name="t8">An argument to pass to the func.</param>
    /// <param name="t9">An argument to pass to the func.</param>
    /// <param name="t10">An argument to pass to the func.</param>
    /// <param name="t11">An argument to pass to the func.</param>
    /// <param name="t12">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task>? func,
        T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12
    ) =>
        func?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <typeparam name="T4">An argument type.</typeparam>
    /// <typeparam name="T5">An argument type.</typeparam>
    /// <typeparam name="T6">An argument type.</typeparam>
    /// <typeparam name="T7">An argument type.</typeparam>
    /// <typeparam name="T8">An argument type.</typeparam>
    /// <typeparam name="T9">An argument type.</typeparam>
    /// <typeparam name="T10">An argument type.</typeparam>
    /// <typeparam name="T11">An argument type.</typeparam>
    /// <typeparam name="T12">An argument type.</typeparam>
    /// <typeparam name="T13">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <param name="t4">An argument to pass to the func.</param>
    /// <param name="t5">An argument to pass to the func.</param>
    /// <param name="t6">An argument to pass to the func.</param>
    /// <param name="t7">An argument to pass to the func.</param>
    /// <param name="t8">An argument to pass to the func.</param>
    /// <param name="t9">An argument to pass to the func.</param>
    /// <param name="t10">An argument to pass to the func.</param>
    /// <param name="t11">An argument to pass to the func.</param>
    /// <param name="t12">An argument to pass to the func.</param>
    /// <param name="t13">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task>? func,
        T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13
    ) =>
        func?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <typeparam name="T4">An argument type.</typeparam>
    /// <typeparam name="T5">An argument type.</typeparam>
    /// <typeparam name="T6">An argument type.</typeparam>
    /// <typeparam name="T7">An argument type.</typeparam>
    /// <typeparam name="T8">An argument type.</typeparam>
    /// <typeparam name="T9">An argument type.</typeparam>
    /// <typeparam name="T10">An argument type.</typeparam>
    /// <typeparam name="T11">An argument type.</typeparam>
    /// <typeparam name="T12">An argument type.</typeparam>
    /// <typeparam name="T13">An argument type.</typeparam>
    /// <typeparam name="T14">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <param name="t4">An argument to pass to the func.</param>
    /// <param name="t5">An argument to pass to the func.</param>
    /// <param name="t6">An argument to pass to the func.</param>
    /// <param name="t7">An argument to pass to the func.</param>
    /// <param name="t8">An argument to pass to the func.</param>
    /// <param name="t9">An argument to pass to the func.</param>
    /// <param name="t10">An argument to pass to the func.</param>
    /// <param name="t11">An argument to pass to the func.</param>
    /// <param name="t12">An argument to pass to the func.</param>
    /// <param name="t13">An argument to pass to the func.</param>
    /// <param name="t14">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task>? func,
        T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14
    ) =>
        func?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <typeparam name="T4">An argument type.</typeparam>
    /// <typeparam name="T5">An argument type.</typeparam>
    /// <typeparam name="T6">An argument type.</typeparam>
    /// <typeparam name="T7">An argument type.</typeparam>
    /// <typeparam name="T8">An argument type.</typeparam>
    /// <typeparam name="T9">An argument type.</typeparam>
    /// <typeparam name="T10">An argument type.</typeparam>
    /// <typeparam name="T11">An argument type.</typeparam>
    /// <typeparam name="T12">An argument type.</typeparam>
    /// <typeparam name="T13">An argument type.</typeparam>
    /// <typeparam name="T14">An argument type.</typeparam>
    /// <typeparam name="T15">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <param name="t4">An argument to pass to the func.</param>
    /// <param name="t5">An argument to pass to the func.</param>
    /// <param name="t6">An argument to pass to the func.</param>
    /// <param name="t7">An argument to pass to the func.</param>
    /// <param name="t8">An argument to pass to the func.</param>
    /// <param name="t9">An argument to pass to the func.</param>
    /// <param name="t10">An argument to pass to the func.</param>
    /// <param name="t11">An argument to pass to the func.</param>
    /// <param name="t12">An argument to pass to the func.</param>
    /// <param name="t13">An argument to pass to the func.</param>
    /// <param name="t14">An argument to pass to the func.</param>
    /// <param name="t15">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task>? func,
        T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15
    ) =>
        func?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) ?? Task.CompletedTask;

    /// <summary>
    /// Invoke the given func if it's not null.
    /// </summary>
    /// <typeparam name="T1">An argument type.</typeparam>
    /// <typeparam name="T2">An argument type.</typeparam>
    /// <typeparam name="T3">An argument type.</typeparam>
    /// <typeparam name="T4">An argument type.</typeparam>
    /// <typeparam name="T5">An argument type.</typeparam>
    /// <typeparam name="T6">An argument type.</typeparam>
    /// <typeparam name="T7">An argument type.</typeparam>
    /// <typeparam name="T8">An argument type.</typeparam>
    /// <typeparam name="T9">An argument type.</typeparam>
    /// <typeparam name="T10">An argument type.</typeparam>
    /// <typeparam name="T11">An argument type.</typeparam>
    /// <typeparam name="T12">An argument type.</typeparam>
    /// <typeparam name="T13">An argument type.</typeparam>
    /// <typeparam name="T14">An argument type.</typeparam>
    /// <typeparam name="T15">An argument type.</typeparam>
    /// <typeparam name="T16">An argument type.</typeparam>
    /// <param name="func">The func to invoke.</param>
    /// <param name="t1">An argument to pass to the func.</param>
    /// <param name="t2">An argument to pass to the func.</param>
    /// <param name="t3">An argument to pass to the func.</param>
    /// <param name="t4">An argument to pass to the func.</param>
    /// <param name="t5">An argument to pass to the func.</param>
    /// <param name="t6">An argument to pass to the func.</param>
    /// <param name="t7">An argument to pass to the func.</param>
    /// <param name="t8">An argument to pass to the func.</param>
    /// <param name="t9">An argument to pass to the func.</param>
    /// <param name="t10">An argument to pass to the func.</param>
    /// <param name="t11">An argument to pass to the func.</param>
    /// <param name="t12">An argument to pass to the func.</param>
    /// <param name="t13">An argument to pass to the func.</param>
    /// <param name="t14">An argument to pass to the func.</param>
    /// <param name="t15">An argument to pass to the func.</param>
    /// <param name="t16">An argument to pass to the func.</param>
    /// <returns>
    /// The task returned by the func if it's not null, otherwise a completed
    /// task.
    /// </returns>
    public static Task TryInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task>? func,
        T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16
    ) =>
        func?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) ?? Task.CompletedTask;
}
