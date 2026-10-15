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

namespace SingleFinite.Essentials.UnitTests;

[TestClass]
public class ExceptionHandlerTests
{
    [TestMethod]
    public void Combine_Of_Null_Or_Empty_List_Returns_Null()
    {
        var emptyList = new List<IExceptionHandler>();
        var emptyListHandler = emptyList.CombineHandlers();
        Assert.IsNull(emptyListHandler);

        var nullsList = (List<IExceptionHandler?>)[null, null];
        var nullsListHandler = nullsList.CombineHandlers();
        Assert.IsNull(nullsListHandler);
    }

    [TestMethod]
    public void Combine_Of_Single_Handler_Returns_Same_Handler()
    {
        var handler = new ExceptionHandlerObserver(handleReturn: true);
        var list = (List<IExceptionHandler>)[handler];
        var listHandler = list.CombineHandlers();
        Assert.AreSame(handler, listHandler);
    }

    [TestMethod]
    public void Combine_Handle_Moves_Up_List()
    {
        var handler0 = new ExceptionHandlerObserver(handleReturn: false);
        var handler1 = new ExceptionHandlerObserver(handleReturn: false);
        var handler2 = new ExceptionHandlerObserver(handleReturn: true);
        var handler3 = new ExceptionHandlerObserver(handleReturn: false);
        var handlerList = (List<IExceptionHandler>)
        [
            handler0,
            handler1,
            handler2,
            handler3
        ];

        var handler = handlerList.CombineHandlers();
        Assert.IsNotNull(handler);

        var handleResult = handler.Handle(
            exception: new InvalidOperationException("Example error"),
            message: "Example message"
        );

        Assert.IsTrue(handleResult);

        Assert.HasCount(1, handler0.Entries);
        Assert.IsInstanceOfType<InvalidOperationException>(handler0.Entries[0].Exception);
        Assert.AreEqual("Example error", handler0.Entries[0].Exception?.Message);
        Assert.AreEqual("Example message", handler0.Entries[0].Message);

        Assert.HasCount(1, handler1.Entries);
        Assert.IsInstanceOfType<InvalidOperationException>(handler1.Entries[0].Exception);
        Assert.AreEqual("Example error", handler1.Entries[0].Exception?.Message);
        Assert.AreEqual("Example message", handler1.Entries[0].Message);

        Assert.HasCount(1, handler2.Entries);
        Assert.IsInstanceOfType<InvalidOperationException>(handler2.Entries[0].Exception);
        Assert.AreEqual("Example error", handler2.Entries[0].Exception?.Message);
        Assert.AreEqual("Example message", handler2.Entries[0].Message);

        Assert.IsEmpty(handler3.Entries);
    }

    private class ExceptionHandlerObserver(bool handleReturn) : IExceptionHandler
    {
        public List<Entry> Entries { get; } = [];

        public bool Handle(Exception? exception, string? message = null)
        {
            Entries.Add(new(exception, message));
            return handleReturn;
        }

        public record Entry(Exception? Exception, string? Message);
    }
}
