﻿#pragma checksum "..\..\..\Setup.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "AC33F22B73E5A4B763C10AB70F462B166E879072"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// Setup
    /// </summary>
    public partial class Setup : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 43 "..\..\..\Setup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox InWord1;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\Setup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox InWord2;
        
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
            System.Uri resourceLocater = new System.Uri("/RobotWeld2;component/setup.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Setup.xaml"
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
            this.InWord1 = ((System.Windows.Controls.TextBox)(target));
            
            #line 47 "..\..\..\Setup.xaml"
            this.InWord1.GotFocus += new System.Windows.RoutedEventHandler(this.InWord1_GotFocus);
            
            #line default
            #line hidden
            
            #line 48 "..\..\..\Setup.xaml"
            this.InWord1.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.InWord1_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 49 "..\..\..\Setup.xaml"
            this.InWord1.KeyDown += new System.Windows.Input.KeyEventHandler(this.InWord1_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.InWord2 = ((System.Windows.Controls.TextBox)(target));
            
            #line 67 "..\..\..\Setup.xaml"
            this.InWord2.GotFocus += new System.Windows.RoutedEventHandler(this.InWord2_GotFocus);
            
            #line default
            #line hidden
            
            #line 68 "..\..\..\Setup.xaml"
            this.InWord2.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.InWord2_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 69 "..\..\..\Setup.xaml"
            this.InWord2.KeyDown += new System.Windows.Input.KeyEventHandler(this.InWord2_KeyDown);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 76 "..\..\..\Setup.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CancelButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 78 "..\..\..\Setup.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OkButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

