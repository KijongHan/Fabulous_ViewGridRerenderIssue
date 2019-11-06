// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace TestOpenPdf_OnFabulous

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
open Xamarin.Essentials
open System
open System.IO

module App = 
    type Model = 
      { RenderButton: bool }

    type Msg = | RenderButton

    let initModel = { RenderButton = false }

    let init () = initModel, Cmd.none

    let update msg model =
        match msg with
        | RenderButton ->
            { model with RenderButton = true }, Cmd.none

    let view (model: Model) dispatch =
        View.ContentPage(
            content = View.Grid(
                coldefs = [Star; Stars 25.; Star],
                rowdefs = [Auto; Auto; Auto; Auto; Auto; Auto],
                children = [
                    yield View.Label(text = "Test Label").Row(0).Column(1)
                    yield View.Label(text = "Test Label").Row(1).Column(1)
                    if model.RenderButton then
                        yield View.Button(text = "Test Button").Row(2).Column(1)
                    else
                        yield View.Label(text = "Test Label").Row(2).Column(1)
                    yield View.Label(text = "Test Label").Row(3).Column(1)
                    yield View.Label(text = "Test Label").Row(4).Column(1)
                    yield View.Button(text = "Render", command = (fun () -> dispatch RenderButton)).Row(5).Column(1)
                ]
            )
        )

    // Note, this declaration is needed if you enable LiveUpdate
    let program = Program.mkProgram init update view

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> XamarinFormsProgram.run app

#if DEBUG
    // Uncomment this line to enable live update in debug mode. 
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/tools.html#live-update for further  instructions.
    //
    //do runner.EnableLiveUpdate()
#endif    

    // Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/models.html#saving-application-state for further  instructions.
#if APPSAVE
    let modelId = "model"
    override __.OnSleep() = 

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() = 
        Console.WriteLine "OnResume: checking for model in app.Properties"
        try 
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) -> 

                Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex -> 
            App.program.onError("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Console.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()
#endif


