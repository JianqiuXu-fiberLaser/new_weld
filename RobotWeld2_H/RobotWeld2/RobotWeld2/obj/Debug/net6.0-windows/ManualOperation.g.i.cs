﻿#pragma checksum "..\..\..\ManualOperation.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "BA5717B4224D87A347044DBE4D8DB07B9B86E8B2"
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
    /// ManualOperation
    /// </summary>
    public partial class ManualOperation : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 73 "..\..\..\ManualOperation.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox SWaddrees;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\..\ManualOperation.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button LeftClip;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\ManualOperation.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button LeftLocation;
        
        #line default
        #line hidden
        
        
        #line 138 "..\..\..\ManualOperation.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RightClip;
        
        #line default
        #line hidden
        
        
        #line 143 "..\..\..\ManualOperation.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RightLocation;
        
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
            System.Uri resourceLocater = new System.Uri("/RobotWeld2;V1.0.0.0;component/manualoperation.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ManualOperation.xaml"
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
            this.SWaddrees = ((System.Windows.Controls.TextBox)(target));
            
            #line 75 "..\..\..\ManualOperation.xaml"
            this.SWaddrees.GotFocus += new System.Windows.RoutedEventHandler(this.SWaddrees_GotFocus);
            
            #line default
            #line hidden
            
            #line 76 "..\..\..\ManualOperation.xaml"
            this.SWaddrees.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.SWaddrees_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 77 "..\..\..\ManualOperation.xaml"
            this.SWaddrees.KeyDown += new System.Windows.Input.KeyEventHandler(this.SWaddrees_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.LeftClip = ((System.Windows.Controls.Button)(target));
            
            #line 91 "..\..\..\ManualOperation.xaml"
            this.LeftClip.Click += new System.Windows.RoutedEventHandler(this.LeftClip_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.LeftLocation = ((System.Windows.Controls.Button)(target));
            
            #line 96 "..\..\..\ManualOperation.xaml"
            this.LeftLocation.Click += new System.Windows.RoutedEventHandler(this.LeftLocation_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.RightClip = ((System.Windows.Controls.Button)(target));
            
            #line 140 "..\..\..\ManualOperation.xaml"
            this.RightClip.Click += new System.Windows.RoutedEventHandler(this.RightClip_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.RightLocation = ((System.Windows.Controls.Button)(target));
            
            #line 145 "..\..\..\ManualOperation.xaml"
            this.RightLocation.Click += new System.Windows.RoutedEventHandler(this.RightLocation_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

