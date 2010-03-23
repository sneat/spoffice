<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%@ Import Namespace="ScriptManager" %>

<% Html.ScriptManager().ScriptInclude("jquery", "~/Scripts/jquery-latest.min.js"); %>
<% Html.ScriptManager().ScriptInclude("jixed", "~/Scripts/jixedbar-0.0.2.js"); %>
<% Html.ScriptManager().Script( "addFavourite", () => {%>
    var cacheImage = document.createElement('img');
    cacheImage.src = "<%= ResolveUrl("~/Content/images/ajax-loader.gif") %>";
    cacheImage.setAttribute('alt', '<%= ViewRes.FavouritesStrings.PleaseWait %>');
    
    $('a[title="<%= ViewRes.MusicStrings.AddFavourites %>"]').click(function(e) {
        e.preventDefault();
        $(this).replaceWith(cacheImage);
        $(document.createElement('p')).addClass('result').appendTo("body");
        $.post('/Favourites/Add', {
                id: $(this).attr('id')
            }, function(data) {
                if (data)
                {
                    if (data.Status == 'OK')
                    {
                        $('p.result').addClass('success').text('<%= ViewRes.FavouritesStrings.SavedFavourites %>').jixedbar();
                        $(cacheImage).replaceWith($(document.createElement('img')).attr('src', '<%= ResolveUrl("~/Content/images/accept.png") %>').attr('title', '<%= ViewRes.FavouritesStrings.AlreadyFavourite %>'));
                    }
                    else
                    {
                        $('p.result').addClass('error').text(data.Message).jixedbar();
                    }
                }
                else
                {
                    $('p.result').addClass('error').text('<%= ViewRes.FavouritesStrings.ErrorSaving %>').jixedbar();
                }
        }, 'json');
    });
    
<%}); %>

<%= Html.ActionLink("[imgtag]", "Add", "Favourites", new { id = Model }, new { title = ViewRes.MusicStrings.AddFavourites, id = Model }).Replace("[imgtag]", "<img src=\"" + ResolveUrl("~/Content/images/add.png") + "\" alt=\"" + ViewRes.MusicStrings.AddFavourites + "\" />")%>