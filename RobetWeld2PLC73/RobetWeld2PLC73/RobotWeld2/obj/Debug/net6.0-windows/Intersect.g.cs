﻿#pragma checksum "..\..\..\Intersect.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "E9C40E99ED5619D9003DEB649A8D4B9EF13679AC"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using RobotWeld2;
using RobotWeld2.ViewModel;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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


namespace RobotWeld2 {
    
    
    /// <summary>
    /// Intersect
    /// </summary>
    public partial class Intersect : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 86 "..\..\..\Intersect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox VerticalTube;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\Intersect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox HorizonalTube;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.4.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/RobotWeld2;component/intersect.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Intersect.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.4.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.VerticalTube = ((System.Windows.Controls.TextBox)(target));
            
            #line 86 "..\..\..\Intersect.xaml"
            this.VerticalTube.GotFocus += new System.Windows.RoutedEventHandler(this.VerticalTube_GotFocus);
            
            #line default
            #line hidden
            
            #line 86 "..\..\..\Intersect.xaml"
            this.VerticalTube.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.VerticalTube_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 86 "..\..\..\Intersect.xaml"
            this.VerticalTube.KeyDown += new System.Windows.Input.KeyEventHandler(this.VerticalTube_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.HorizonalTube = ((System.Windows.Controls.TextBox)(target));
            
            #line 94 "..\..\..\Intersect.xaml"
            this.HorizonalTube.GotFocus += new System.Windows.RoutedEventHandler(this.HorizonalTube_GotFocus);
            
            #line default
            #line hidden
            
            #line 94 "..\..\..\Intersect.xaml"
            this.HorizonalTube.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.HorizonalTube_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 94 "..\..\..\Intersect.xaml"
            this.HorizonalTube.KeyDown += new System.Windows.Input.KeyEventHandler(this.HorizonalTube_KeyDown);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 104 "..\..\..\Intersect.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CancelButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 106 "..\..\..\Intersect.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OkButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

