#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;

namespace Kooboo.HealthMonitoring
{
    #region WebBaseErrorEventWrapper
    /// <summary>
    /// 
    /// </summary>
    public class WebBaseErrorEventWrapper : WebBaseEventWrapper
    {
        public WebBaseErrorEventWrapper(string message, object eventSource, int eventCode, Exception e)
            : base(message, eventSource, eventCode)
        {
            this.Init(e);
        }
        private void Init(Exception e)
        {
            this._exception = e;
        }
        private Exception _exception;
        public Exception ErrorException
        {
            get
            {
                return this._exception;
            }
        }

        internal override void GenerateFieldsForMarshal(List<WebEventFieldData> fields)
        {
            base.GenerateFieldsForMarshal(fields);

            fields.Add(new WebEventFieldData("ExceptionType", this.ErrorException.GetType().ToString(), WebEventFieldType.String));
            fields.Add(new WebEventFieldData("ExceptionMessage", this.ErrorException.Message, WebEventFieldType.String));

        }

        protected override void FormatToString(WebEventFormatter formatter, bool includeAppInfo)
        {
            base.FormatToString(formatter, includeAppInfo);
            if (this._exception != null)
            {
                Exception innerException = this._exception;
                for (int i = 0; (innerException != null) && (i <= 2); i++)
                {
                    formatter.AppendLine(string.Empty);
                    if (i == 0)
                    {
                        formatter.AppendLine(WebBaseEventWrapper.FormatResourceStringWithCache("Webevent_event_exception_information"));
                    }
                    else
                    {
                        formatter.AppendLine(WebBaseEventWrapper.FormatResourceStringWithCache("Webevent_event_inner_exception_information", i.ToString(CultureInfo.InstalledUICulture)));
                    }
                    formatter.IndentationLevel++;
                    formatter.AppendLine(WebBaseEventWrapper.FormatResourceStringWithCache("Webevent_event_exception_type", innerException.GetType().ToString()));
                    formatter.AppendLine(WebBaseEventWrapper.FormatResourceStringWithCache("Webevent_event_exception_message", innerException.Message));
                    formatter.IndentationLevel--;
                    innerException = innerException.InnerException;
                }
            }

        }
    }
    #endregion

    #region WebRequestErrorEventWrapper

    /// <summary>
    /// 
    /// </summary>
    public class WebRequestErrorEventWrapper : WebBaseErrorEventWrapper
    {
        public WebRequestErrorEventWrapper(string message, object eventSource, int eventCode, Exception e)
            : base(message, eventSource, eventCode, e)
        {
        }

        protected override void FormatToString(WebEventFormatter formatter, bool includeAppInfo)
        {
            base.FormatToString(formatter, includeAppInfo);
            formatter.AppendLine(string.Empty);
            formatter.AppendLine(WebBaseEventWrapper.FormatResourceStringWithCache("Webevent_event_request_information"));
            formatter.IndentationLevel++;
            this.RequestInformation.FormatToString(formatter);
            formatter.IndentationLevel--;
            formatter.AppendLine(string.Empty);
            formatter.AppendLine(WebBaseEventWrapper.FormatResourceStringWithCache("Webevent_event_thread_information"));
            formatter.IndentationLevel++;
            this.ThreadInformation.FormatToString(formatter);
            formatter.IndentationLevel--;

        }
        internal override void GenerateFieldsForMarshal(List<WebEventFieldData> fields)
        {
            base.GenerateFieldsForMarshal(fields);
            fields.Add(new WebEventFieldData("RequestUrl", this.RequestInformation.RequestUrl, WebEventFieldType.String));
            fields.Add(new WebEventFieldData("RequestPath", this.RequestInformation.RequestPath, WebEventFieldType.String));
            fields.Add(new WebEventFieldData("UserHostAddress", this.RequestInformation.UserHostAddress, WebEventFieldType.String));
            fields.Add(new WebEventFieldData("UserName", this.RequestInformation.Principal.Identity.Name, WebEventFieldType.String));
            fields.Add(new WebEventFieldData("UserAuthenticated", this.RequestInformation.Principal.Identity.IsAuthenticated.ToString(), WebEventFieldType.Bool));
            fields.Add(new WebEventFieldData("UserAuthenticationType", this.RequestInformation.Principal.Identity.AuthenticationType, WebEventFieldType.String));
            fields.Add(new WebEventFieldData("RequestThreadAccountName", this.RequestInformation.ThreadAccountName, WebEventFieldType.String));
            fields.Add(new WebEventFieldData("ThreadID", this.ThreadInformation.ThreadID.ToString(CultureInfo.InstalledUICulture), WebEventFieldType.Int));
            fields.Add(new WebEventFieldData("ThreadAccountName", this.ThreadInformation.ThreadAccountName, WebEventFieldType.String));
            fields.Add(new WebEventFieldData("StackTrace", this.ThreadInformation.StackTrace, WebEventFieldType.String));
            fields.Add(new WebEventFieldData("IsImpersonating", this.ThreadInformation.IsImpersonating.ToString(), WebEventFieldType.Bool));
        }

        public WebRequestInformation RequestInformation
        {
            get
            {
                this.InitRequestInformation();
                return this._requestInfo;
            }
        }
        private void InitRequestInformation()
        {
            if (this._requestInfo == null)
            {
                this._requestInfo = new WebRequestInformation();
            }
        }
        public WebThreadInformation ThreadInformation
        {
            get
            {
                this.InitThreadInformation();
                return this._threadInfo;
            }
        }
        private void InitThreadInformation()
        {
            if (this._threadInfo == null)
            {
                this._threadInfo = new WebThreadInformation(base.ErrorException);
            }
        }

        private WebThreadInformation _threadInfo;
        private WebRequestInformation _requestInfo;
    }
    #endregion
}