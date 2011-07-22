Imports System
Imports System.Xml.Linq
Imports MiniMVC

Public Module Views

    Public Function StaticUrl(ByVal resource As String) As String
        Return String.Format("static.ashx?r={0}", resource)
    End Function

    Public Function IndexHead() As XElement
        Return _
            <x>
                <title>Boo console</title>
                <style type="text/css">
        		.error {
        		    color: red;
        		}
        		</style>
                <link rel="Stylesheet" href=<%= StaticUrl("jquery.autocomplete.css") %> type="text/css"/>
                <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" type="text/javascript"></script>
                <script src=<%= StaticUrl("jquery.autocomplete.js") %> type="text/javascript"></script>
            </x>
    End Function

    Public Function IndexDocument(ByVal model As Context) As XDocument
        Return New XDocument(X.XHTML1_0_Transitional, Index(model))
    End Function

    Public Function Index(ByVal model As Context) As XElement
        Return _
        <html>
            <head>
                <%= IndexHead().Nodes() %>
            </head>
            <body>
                <form method="post" action="">
                    <textarea name="prg" cols="80" rows="10" accesskey="e" id="prg"><%= If(model.Prg, "") %></textarea><br/>
                    <input type="submit" value="Execute" accesskey="x"/>
                </form>
                <%= If(Not String.IsNullOrEmpty(model.Errors),
                    <pre class="error"><%= model.Errors %></pre>,
                    Nothing) %>
                <%= If(Not String.IsNullOrEmpty(model.Output),
                    <pre><%= model.Output %></pre>,
                    Nothing) %>
                <script type="text/javascript">
                //<![CDATA[
        	    		function getPosition(textarea) {
        	    				var a = document.getElementById(textarea);
        							a.focus();
        							return a.selectionStart;
        	    		}

        	    		function insertString(a, pos, b) {
        	    			return a.substr(0, pos) + b + a.substr(pos);
        	    		}

        	        jQuery('#prg').autocomplete('suggest.ashx', {
        	            matchCase: true,
                        dataType:  'json',
        	            beforeSend: function(r) {
        	            	var pos = getPosition('prg');
        	            	return insertString(r, pos, '__codecomplete__');
        	            },
        	            formatItem: function(r) { return r; },
        	            parse: function(data) {
        	            	var v = jQuery('#prg').val();
        	                return jQuery.map(data, function(r) {
        	                    return {
        	                        data: r,
        	                        value: v + r,
        	                        result: v + r
        	                    };
        	                });
        	            }
        	        });
        	    //]]>
                </script>
            </body>
        </html>
    End Function
End Module
