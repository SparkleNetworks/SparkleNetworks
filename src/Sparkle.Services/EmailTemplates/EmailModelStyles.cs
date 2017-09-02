using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparkle.Services.EmailTemplates
{
    public class EmailModelStyles
    {
        private EmailModelTableStyle tables;
        private EmailModelColors colors;

        public EmailModelStyles(string networkAccentColor)
        {
            this.StyleH1 = "color:#777; display: block;text-shadow:0 1px 0 rgba(255, 255, 255, 0.8)font-family: Arial;font-size: 24px;font-weight: lighter;line-height: 100%;margin-top: 0;margin-right: 0;margin-bottom: 10px;margin-left: 0;text-align: left;";

            this.StyleH2 = "color:#444;font-size:28px;font-weight:200;text-shadow:0 1px 0 rgba(255, 255, 255, 0.8)";

            this.StyleH3 = "color:#444;font-size:20px;font-weight:200;text-shadow:0 1px 0 rgba(255, 255, 255, 0.8)";

            this.StyleH4 = "color:#444;font-size:17px;font-weight:200;text-shadow:0 1px 0 rgba(255, 255, 255, 0.8)";

            this.StyleHR = "border:0;border-top:1px solid #e3e3e3;border-bottom:1px solid #f7f7f7;margin:10px 0;";

            this.StyleTour = "color:#777;font-size:14px;text-align:justify;text-shadow:0 1px 0 rgba(255, 255, 255, 0.8)";

            this.StyleLink = "color:" + networkAccentColor + ";text-decoration:none;";

            this.StyleButton = "background-color:" + networkAccentColor + "; text-decoration:none; display:inline-block; border:0; font-size:16px; margin: auto; height:35px; line-height: 35px; padding: 0 30px; color:#fff; text-shadow:none;";

            this.StyleAvatar = "float:left; margin:0 10px 0 10px; background-color:#f2f2f2; border:1px solid #fff; width:50px; height:50px;";

            this.StyleContent = "padding: 20px; font-size:17px";

            this.StyleQuote = "margin:0 0 10px 0px; border-left:4px solid #DDD; color: #777; padding: 0 15 px";

            this.PersonColor = "#0e9d43";
            this.PersonBackground = "#e1ede3";
            this.CompanyColor = "#ff2a3b";
            this.CompanyBackground = "#ecdce8";
            this.EventColor = "#f4890a";
            this.GroupColor = "#1774a4";

            this.FooterBackground = "#E7E7E7";

            this.Hello = "Bonjour ";
            this.HaveANiceDay = "Bonne journée,";
        }

        public string StyleH1 { get; set; }

        public string StyleH2 { get; set; }

        public string StyleH3 { get; set; }

        public string StyleH4 { get; set; }

        public string StyleHR { get; set; }

        public string StyleTour { get; set; }

        public string StyleLink { get; set; }

        public string StyleButton { get; set; }

        public string StyleAvatar { get; set; }

        public string StyleQuote { get; set; }

        public string Hello { get; set; }

        public string HaveANiceDay { get; set; }

        public string PersonColor { get; set; }

        public string CompanyColor { get; set; }

        public string EventColor { get; set; }

        public string GroupColor { get; set; }

        public string StyleContent { get; set; }

        public string PersonBackground { get; set; }

        public string CompanyBackground { get; set; }

        public string FooterBackground { get; set; }

        public EmailModelTableStyle Tables
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.tables ?? (this.tables = new EmailModelTableStyle()); }
        }

        public EmailModelColors Colors
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.colors ?? (this.colors = new EmailModelColors()); }
        }
    }

    public class EmailModelTableStyle
    {
        public string TableStyle
        {
            get { return "border-collapse: collapse; border-spacing: 0; text-align: left; "; }
        }

        public string TheadStyle
        {
            get { return "border: none; "; }
        }

        public string TheadTrStyle
        {
            get { return "background:lightgray; color: #000; "; }
        }

        public string TheadThStyle
        {
            get { return "border: none;font-size:1.1em; padding: 10px; "; }
        }

        public string TbodyStyle
        {
            get { return "border: none; "; }
        }

        public string TbodyTrStyle
        {
            get { return "background-color: rgba(255,255,255,.02); "; }
        }

        public string TbodyThStyle
        {
            get { return "border: none; padding: 10px; font-weight: lighter; vertical-align: middle; "; }
        }

        public string TbodyTdStyle
        {
            get { return "border: none; padding: 10px; font-weight: lighter; vertical-align: middle; "; }
        }
    }

    public class EmailModelColors
    {
        public string DashboardColor = "#ea246a";
        public string DashboardBackground { get { return "background-color: " + DashboardColor + "; color:white; "; } }
    }
}
