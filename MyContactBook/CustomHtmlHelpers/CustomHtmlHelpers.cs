using System.Web;
using System.Web.Mvc;

namespace MyContactBook.CustomHtmlHelpers
{
    public static class CustomHtmlHelpers
    {
        public static IHtmlString Image(this HtmlHelper helper, string src, string alt)
        {
            TagBuilder tagBuilder = new TagBuilder("img");
            tagBuilder.Attributes.Add("src", VirtualPathUtility.ToAbsolute(src));
            tagBuilder.Attributes.Add("alt", alt);
            //tagBuilder.AddCssClass("img-responsive");
            tagBuilder.Attributes.Add("class", "img-responsive");
            tagBuilder.Attributes.Add("style","width:100px ; height:60px");
           return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.SelfClosing));  //since img is a self closing tag

        }

    }

}