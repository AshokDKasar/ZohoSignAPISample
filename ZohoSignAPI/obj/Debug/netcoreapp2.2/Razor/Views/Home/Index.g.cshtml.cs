#pragma checksum "E:\Ashok-zohoApiSampleProject\ZohoSignAPISample\ZohoSignAPI\Views\Home\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ee987fe18a91424b7c5e51e890dbe6b1535d1611"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Index), @"mvc.1.0.view", @"/Views/Home/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/Index.cshtml", typeof(AspNetCore.Views_Home_Index))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "E:\Ashok-zohoApiSampleProject\ZohoSignAPISample\ZohoSignAPI\Views\_ViewImports.cshtml"
using ZohoSignAPI;

#line default
#line hidden
#line 2 "E:\Ashok-zohoApiSampleProject\ZohoSignAPISample\ZohoSignAPI\Views\_ViewImports.cshtml"
using ZohoSignAPI.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ee987fe18a91424b7c5e51e890dbe6b1535d1611", @"/Views/Home/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"da533af567b59c1e5f0d8d76c42072ec054335e4", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 1 "E:\Ashok-zohoApiSampleProject\ZohoSignAPISample\ZohoSignAPI\Views\Home\Index.cshtml"
  
    ViewData["Title"] = "Home Page";

#line default
#line hidden
            BeginContext(45, 29, true);
            WriteLiteral("\r\n<div class=\"text-center\">\r\n");
            EndContext();
#line 6 "E:\Ashok-zohoApiSampleProject\ZohoSignAPISample\ZohoSignAPI\Views\Home\Index.cshtml"
     if (TempData["Success"] != null)
    {

#line default
#line hidden
            BeginContext(120, 58, true);
            WriteLiteral("        <div    id=\"successMessage\">\r\n            <strong>");
            EndContext();
            BeginContext(179, 19, false);
#line 9 "E:\Ashok-zohoApiSampleProject\ZohoSignAPISample\ZohoSignAPI\Views\Home\Index.cshtml"
               Write(TempData["Success"]);

#line default
#line hidden
            EndContext();
            BeginContext(198, 197, true);
            WriteLiteral("</strong>\r\n            <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">\r\n                <span aria-hidden=\"true\">&times;</span>\r\n            </button>\r\n        </div>\r\n");
            EndContext();
#line 14 "E:\Ashok-zohoApiSampleProject\ZohoSignAPISample\ZohoSignAPI\Views\Home\Index.cshtml"
    }

#line default
#line hidden
            BeginContext(402, 74, true);
            WriteLiteral("\r\n    <input type=\"submit\" formaction=\"SendTemplate\" value=\"Send Template\"");
            EndContext();
            BeginWriteAttribute("onclick", " onclick=\"", 476, "\"", 551, 4);
            WriteAttributeValue("", 486, "location.href=\'", 486, 15, true);
#line 16 "E:\Ashok-zohoApiSampleProject\ZohoSignAPISample\ZohoSignAPI\Views\Home\Index.cshtml"
WriteAttributeValue("", 501, Url.Action("SendTemplate", "Home"), 501, 35, false);

#line default
#line hidden
            WriteAttributeValue("", 536, "\';return", 536, 8, true);
            WriteAttributeValue(" ", 544, "false;", 545, 7, true);
            EndWriteAttribute();
            BeginContext(552, 13, true);
            WriteLiteral(" />\r\n</div>\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
