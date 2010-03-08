// Learn more about F# at http://fsharp.net


namespace UltraApplication2


// http://en.wikibooks.org/wiki/F_Sharp_Programming/Modules_and_Namespaces
open global.ScriptCoreLib.JavaScript.DOM
open global.ScriptCoreLib.JavaScript.DOM.HTML
open global.ScriptCoreLib.JavaScript
open global.ScriptCoreLib.JavaScript.Extensions
open global.System
open global.System.Reflection
open global.ScriptCoreLib.ActionScript.flash.display
open global.ScriptCoreLib.ActionScript.Extensions
open global.UltraApplication2.Assets1

// http://stackoverflow.com/questions/2269625/using-assembly-attributes-in-f
[<assembly: AssemblyTitleAttribute("Ultra Application")>] 
[<assembly: AssemblyDescriptionAttribute("Ultra Application. Write javascript, flash and java applets within a F# project. http://jsc-solutions.net")>] 
[<assembly: AssemblyCompanyAttribute("jsc-solutions.net")>] 
do ()
   
[<Sealed>]
 type UltraApplication2(e : IHTMLElement) = do
    Native.Document.title <- "Ultra Application powered by FSharp and jsc"
    let a1 = new MyAssets()
    a1.jsc.style.border <- "1px dashed red";

    Extensions.Extensions.AttachToDocument(a1.Container)|>ignore  

    let ApplicationView = Extensions.Extensions.AttachToDocument( new IHTMLDiv())
    do
        ApplicationView.style.position <- IStyle.PositionEnum.absolute
        ApplicationView.style.width <-"100%"
        ApplicationView.style.height <- "100%"
        ApplicationView.style.overflow <- IStyle.OverflowEnum.auto

    let HelloWorld = new IHTMLButton("FSharp: Hello World")
    do HelloWorld.style.color <- "blue"

    do ApplicationView.appendChild(HelloWorld)
    do HelloWorld.add_onclick(
        fun (e) ->
            let w = new UltraWebService()
            do w.WebMethod1("client fsharp. ",
                fun (y) ->
                    let news = new IHTMLDiv("FSharp: server: " + y)
                    do Native.Document.body.appendChild(
                           news
                    )
            )

    )

    let AddSpriteButton = new IHTMLButton("Add Sprite")
    do AddSpriteButton.style.color <- "blue"
    
    do ApplicationView.appendChild(AddSpriteButton)
    do AddSpriteButton.add_onclick(
        fun (e) ->
            let w = new UltraSprite()
          
            SpriteExtensions.AttachSpriteTo(w, ApplicationView)  |> ignore  

            let AtClickButton = new IHTMLButton("AtClick")
            do ApplicationView.appendChild(AtClickButton)

            do AtClickButton.add_onclick(
                fun (e) ->
                    let news2 = new IHTMLDiv("FSharp: AtClick")
                    
                    do ApplicationView.appendChild(
                               news2
                        )

                    do w.AtClick(
                            fun () ->
                                let news = new IHTMLDiv("FSharp: flash click")
                                do ApplicationView.appendChild(
                                       news
                                )
                        )
            )

    )



    let AddAppletButton = new IHTMLButton("Add Applet")
    do AddAppletButton.style.color <- "blue"
    
    do ApplicationView.appendChild(AddAppletButton)
    do AddAppletButton.add_onclick(
        fun (e) ->
            let w = new UltraApplet()
          
            AppletExtensions.AttachAppletTo(w, ApplicationView)  |> ignore  

          

    )

