using System;
using System.Collections;

namespace uMocks.Exceptions
{
  public class MockNotFoundException : Exception
  {
    private readonly Type _requestedType;

    private readonly IList _currentSetups;

    public MockNotFoundException(Type requestedType, IList currentSetups)
    {
      _requestedType = requestedType;
      _currentSetups = currentSetups;
    }

    public override string Message => GetExceptionMessage();

    private string GetExceptionMessage()
    {
      var message = $"Could not find requested mock in Umbraco factory. Verify your mock session setup.";

      message += "\r\n";
      message += $"Requested mock type: {_requestedType}";
      message += "\r\n";
      message += "\r\n";

      if (_currentSetups.Count == 0)
      {
        message += "There was no additional mock setup found";

        return message;
      }

      message += $"Current factory mock setups ({_currentSetups.Count} item(s)):";
      message += "\r\n";

      foreach (var currentSetup in _currentSetups)
      {
        message += $"- [{currentSetup}]";
        message += "\r\n";
      }

      return message;
    }
  }
}
