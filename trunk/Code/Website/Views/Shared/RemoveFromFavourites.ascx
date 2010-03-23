<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%@ Import Namespace="ScriptManager" %>

<% Html.ScriptManager().ScriptInclude("jquery", "~/Scripts/jquery-latest.min.js"); %>
<% Html.ScriptManager().ScriptInclude("jixed", "~/Scripts/jixedbar-0.0.2.js"); %>
<% Html.ScriptManager().Script( "removeFavourite", () => {%>
    var cacheImage = document.createElement('img');
    cacheImage.src = "<%= ResolveUrl("~/Content/images/ajax-loader.gif") %>";
    cacheImage.setAttribute('alt', '<%= ViewRes.FavouritesStrings.PleaseWait %>');
    
    $('a[title="<%= ViewRes.MusicStrings.RemoveFavourites %>"]').click(function(e) {
        e.preventDefault();
        var form = $(this).closest('form');
        var token = form.find('input[name="__RequestVerificationToken"]').val(); // CSRF token
        $(this).replaceWith(cacheImage);
        $(document.createElement('p')).addClass('result').appendTo("body");
        $.post('/Favourites/Remove', {
                id: form.attr('id'),
                'isJs': true,
                '__RequestVerificationToken': token
            }, function(data) {
                if (data)
                {
                    if (data.Status == 'OK')
                    {
                        $('p.result').addClass('success').text('<%= ViewRes.FavouritesStrings.RemovedFavourite%>').jixedbar().fadeIn().delay(3000).fadeOut("slow", function() { $(this).remove(); });
                        form.closest('tr').remove();
                        if ($('table[summary="Search results"] tbody').find('tr').length == 0)
                        {
                            $('table[summary="Search results"]').replaceWith('<p><%= ViewRes.FavouritesStrings.NoFavourites %></p>');
                        }
                    }
                    else
                    {
                        $('p.result').addClass('error').text(data.Message).jixedbar();
                    }
                }
                else
                {
                    $('p.result').addClass('error').text('<%= ViewRes.FavouritesStrings.ErrorRemoving%>').jixedbar();
                }
        }, 'json');
    });
    
<%}); %>
<% using (Html.BeginForm("Remove", "Favourites", FormMethod.Post, new { id = Model }))
   { %>
    <%= Html.AntiForgeryToken()%>
    <%= Html.ActionLink("[imgtag]", "Remove", "Favourites", new { id = Model }, new { title = ViewRes.MusicStrings.RemoveFavourites }).Replace("[imgtag]", "<img src=\"" + ResolveUrl("~/Content/images/delete.png") + "\" alt=\"" + ViewRes.MusicStrings.RemoveFavourites + "\" />")%>
<% } %>