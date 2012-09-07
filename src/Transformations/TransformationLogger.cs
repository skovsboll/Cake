using System;
using System.Diagnostics;
using Microsoft.Web.Publishing.Tasks;

namespace ConfigTransformationTool.Base
{
	// Simple implementation of logger
	public class TransformationLogger : IXmlTransformationLogger
	{
		#region IXmlTransformationLogger Members

		public void LogMessage(string message, params object[] messageArgs)
		{
			Trace.TraceInformation(message, messageArgs);
		}

		public void LogMessage(MessageType type, string message, params object[] messageArgs)
		{
			Trace.TraceInformation(message, messageArgs);
		}

		public void LogWarning(string message, params object[] messageArgs)
		{
			Trace.TraceWarning(message, messageArgs);
		}

		public void LogWarning(string file, string message, params object[] messageArgs)
		{
			Trace.TraceWarning(string.Format("File: {0}, Message: {1}", file, message), messageArgs);
		}

		public void LogWarning(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
		{
			Trace.TraceWarning(
				string.Format("File: {0}, LineNumber: {1}, LinePosition: {2}, Message: {3}", file, lineNumber, linePosition, message),
				messageArgs);
		}

		public void LogError(string message, params object[] messageArgs)
		{
			Trace.TraceError(message, messageArgs);
		}

		public void LogError(string file, string message, params object[] messageArgs)
		{
			Trace.TraceError(string.Format("File: {0}, Message: {1}", file, message), messageArgs);
		}

		public void LogError(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
		{
			Trace.TraceError(
				string.Format("File: {0}, LineNumber: {1}, LinePosition: {2}, Message: {3}", file, lineNumber, linePosition, message),
				messageArgs);
		}

		public void LogErrorFromException(Exception ex)
		{
			Trace.TraceError(ex.Message, ex);
		}

		public void LogErrorFromException(Exception ex, string file)
		{
			Trace.TraceError(file, ex);
		}

		public void LogErrorFromException(Exception ex, string file, int lineNumber, int linePosition)
		{
			Trace.TraceError(string.Format("File: {0}, LineNumber: {1}, LinePosition: {2}", file, lineNumber, linePosition), ex);
		}

		public void StartSection(string message, params object[] messageArgs)
		{
			Trace.TraceInformation(message, messageArgs);
		}

		public void StartSection(MessageType type, string message, params object[] messageArgs)
		{
			Trace.TraceInformation(message, messageArgs);
		}

		public void EndSection(string message, params object[] messageArgs)
		{
			Trace.TraceInformation(message, messageArgs);
		}

		public void EndSection(MessageType type, string message, params object[] messageArgs)
		{
			Trace.TraceInformation(message, messageArgs);
		}

		#endregion
	}
}