using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.WinForms;
using System.Drawing;
using System.Windows.Forms;

namespace YTPM
{
    public class Player : ChromiumWebBrowser
    {
        public const string HTML = @"<meta http-equiv=""X-UA-Compatible"" content=""IE=edge""/>
 <html>
  <body style=""margin: 0"">
    <div id=""player""></div>

    <script>
      var tag = document.createElement('script');

      tag.src = ""https://www.youtube.com/iframe_api"";
      var firstScriptTag = document.getElementsByTagName('script')[0];
      firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
	  
      var player;

      function onYouTubeIframeAPIReady()
      {
        player = new YT.Player('player', {
          height: '100%',
          width: '100%',
          playerVars: {
            autoplay: 1,
            enablejsapi: 1,
            iv_load_policy: 3,
            modestbranding: 1,
            rel: 0
            },
          events: {
            'onReady': onPlayerReady,
            'onStateChange': onPlayerStateChange
            }
        });
      }

      function onPlayerReady(event) {
        event.target.playVideo();
      }

      function onPlayerStateChange(event) {
        if (event.data == YT.PlayerState.ENDED) {
            interface.nextVideo();
        }
      }
    </script>
  </body>
</html>";

        public Player() : base(string.Empty)
        {
            if (!Cef.IsInitialized) { Cef.Initialize(new CefSettings()); }
            
            Size = new Size(480, 270);

            Dock = DockStyle.None;

            BackColor = Color.Black;
        }
    }
}
