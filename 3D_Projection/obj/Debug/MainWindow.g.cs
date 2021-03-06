﻿#pragma checksum "..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "75F4AAF9780947FEB2391CC10622C88B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using _3D_Projection;
using _3D_Projection.UserControls;


namespace _3D_Projection {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal _3D_Projection.MainWindow Projections;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas ProjectionViewCanvas;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tb;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox inputBox;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid selectGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/3D_Projection;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Projections = ((_3D_Projection.MainWindow)(target));
            return;
            case 2:
            this.ProjectionViewCanvas = ((System.Windows.Controls.Canvas)(target));
            
            #line 28 "..\..\MainWindow.xaml"
            this.ProjectionViewCanvas.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.lmbd_Handler);
            
            #line default
            #line hidden
            
            #line 29 "..\..\MainWindow.xaml"
            this.ProjectionViewCanvas.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.lmbu_Handler);
            
            #line default
            #line hidden
            
            #line 30 "..\..\MainWindow.xaml"
            this.ProjectionViewCanvas.MouseMove += new System.Windows.Input.MouseEventHandler(this.move_Handler);
            
            #line default
            #line hidden
            
            #line 31 "..\..\MainWindow.xaml"
            this.ProjectionViewCanvas.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.rmbd_Handler);
            
            #line default
            #line hidden
            
            #line 32 "..\..\MainWindow.xaml"
            this.ProjectionViewCanvas.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.wheel_Handler);
            
            #line default
            #line hidden
            return;
            case 3:
            this.tb = ((System.Windows.Controls.TextBox)(target));
            
            #line 60 "..\..\MainWindow.xaml"
            this.tb.KeyUp += new System.Windows.Input.KeyEventHandler(this.CaptureInput);
            
            #line default
            #line hidden
            return;
            case 4:
            this.inputBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 71 "..\..\MainWindow.xaml"
            this.inputBox.GotKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(this.EnterSearch);
            
            #line default
            #line hidden
            
            #line 72 "..\..\MainWindow.xaml"
            this.inputBox.KeyUp += new System.Windows.Input.KeyEventHandler(this.CaptureInput);
            
            #line default
            #line hidden
            return;
            case 5:
            this.selectGrid = ((System.Windows.Controls.Grid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

