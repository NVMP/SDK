using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public class GUIWindowElementBuilder
    {
        internal IList<IGUIBaseElement> TargetElementsList;
        internal GUIWindowTemplateBuilder.GUIWindowTemplate TargetWindow;

        public GUIWindowElementBuilder AddButton(Action<GUIButtonElement> configure = null)
        {
            var element = new GUIButtonElement
            {
                ID = ++TargetWindow.NumTotalElements,
                ParentWindow = TargetWindow
            };

            configure?.Invoke(element);

            TargetElementsList.Add(element);
            return this;
        }

        public GUIWindowElementBuilder AddRow(Action<GUIRowElement> configure = null)
        {
            var element = new GUIRowElement
            {
                ID = ++TargetWindow.NumTotalElements,
                ParentWindow = TargetWindow
            };

            configure?.Invoke(element);

            TargetElementsList.Add(element);
            return this;
        }

        public GUIWindowElementBuilder AddColumn(Action<GUIColumnElement> configure = null)
        {
            var element = new GUIColumnElement
            {
                ID = ++TargetWindow.NumTotalElements,
                ParentWindow = TargetWindow
            };

            configure?.Invoke(element);

            TargetElementsList.Add(element);
            return this;
        }

        public GUIWindowElementBuilder AddSeperator(Action<GUISeperatorElement> configure = null)
        {
            var element = new GUISeperatorElement
            {
                ID = ++TargetWindow.NumTotalElements,
                ParentWindow = TargetWindow
            };

            configure?.Invoke(element);

            TargetElementsList.Add(element);
            return this;
        }

        public GUIWindowElementBuilder AddLabel(Action<GUILabelElement> configure = null)
        {
            var element = new GUILabelElement
            {
                ID = ++TargetWindow.NumTotalElements,
                ParentWindow = TargetWindow
            };

            configure?.Invoke(element);

            TargetElementsList.Add(element);
            return this;
        }

        public GUIWindowElementBuilder AddLabel(string label)
        {
            var element = new GUILabelElement
            {
                ID = ++TargetWindow.NumTotalElements,
                ParentWindow = TargetWindow,
            };
            element.WithText(label);
            TargetElementsList.Add(element);
            return this;
        }

        public GUIWindowElementBuilder AddInputText(Action<GUITextInputElement> configure = null)
        {
            var element = new GUITextInputElement
            {
                ID = ++TargetWindow.NumTotalElements,
                ParentWindow = TargetWindow
            };

            configure?.Invoke(element);

            TargetElementsList.Add(element);
            return this;
        }

        public GUIWindowElementBuilder AddCollapsingHeader(Action<GUICollapsingHeaderElement> configure = null)
        {
            var element = new GUICollapsingHeaderElement
            {
                ID = ++TargetWindow.NumTotalElements,
                ParentWindow = TargetWindow
            };

            configure?.Invoke(element);

            TargetElementsList.Add(element);
            return this;
        }

        internal GUIWindowElementBuilder(IList<IGUIBaseElement> targetElementsList, IGUIWindowTemplate targetWindow)
        {
            TargetElementsList = targetElementsList;
            TargetWindow = targetWindow as GUIWindowTemplateBuilder.GUIWindowTemplate;
        }
    }

}
