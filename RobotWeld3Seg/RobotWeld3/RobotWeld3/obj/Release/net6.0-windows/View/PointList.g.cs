﻿#pragma checksum "..\..\..\..\View\PointList.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "DA593AD2AA9A08420CEBEF3F5B32E4AA2E38C4A0"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using RobotWeld3.View;
using RobotWeld3.ViewModel;
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


namespace RobotWeld3.View {
    
    
    /// <summary>
    /// PointList
    /// </summary>
    public partial class PointList : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 30 "..\..\..\..\View\PointList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid GridPointList;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\..\View\PointList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView pointListBox;
        
        #line default
        #line hidden
        
        
        #line 128 "..\..\..\..\View\PointList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ListLaserPower;
        
        #line default
        #line hidden
        
        
        #line 133 "..\..\..\..\View\PointList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ListSpeed;
        
        #line default
        #line hidden
        
        
        #line 138 "..\..\..\..\View\PointList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ListFrequency;
        
        #line default
        #line hidden
        
        
        #line 143 "..\..\..\..\View\PointList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ListDuty;
        
        #line default
        #line hidden
        
        
        #line 150 "..\..\..\..\View\PointList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button DeleteButton;
        
        #line default
        #line hidden
        
        
        #line 151 "..\..\..\..\View\PointList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ChangeButton;
        
        #line default
        #line hidden
        
        
        #line 152 "..\..\..\..\View\PointList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CancelButton;
        
        #line default
        #line hidden
        
        
        #line 153 "..\..\..\..\View\PointList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ConfirmButton;
        
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
            System.Uri resourceLocater = new System.Uri("/RobotWeld3;component/view/pointlist.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\View\PointList.xaml"
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
            
            #line 24 "..\..\..\..\View\PointList.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CommandCanExecute);
            
            #line default
            #line hidden
            
            #line 24 "..\..\..\..\View\PointList.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CommandExecuted);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 25 "..\..\..\..\View\PointList.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CommandCanExecute);
            
            #line default
            #line hidden
            
            #line 25 "..\..\..\..\View\PointList.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CommandExecuted);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 26 "..\..\..\..\View\PointList.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CommandCanExecute);
            
            #line default
            #line hidden
            
            #line 26 "..\..\..\..\View\PointList.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CommandExecuted);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 27 "..\..\..\..\View\PointList.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CommandCanExecute);
            
            #line default
            #line hidden
            
            #line 27 "..\..\..\..\View\PointList.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CommandExecuted);
            
            #line default
            #line hidden
            return;
            case 5:
            this.GridPointList = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.pointListBox = ((System.Windows.Controls.ListView)(target));
            return;
            case 8:
            this.ListLaserPower = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.ListSpeed = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.ListFrequency = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 11:
            this.ListDuty = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 12:
            this.DeleteButton = ((System.Windows.Controls.Button)(target));
            
            #line 150 "..\..\..\..\View\PointList.xaml"
            this.DeleteButton.Click += new System.Windows.RoutedEventHandler(this.ArriveButton_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            this.ChangeButton = ((System.Windows.Controls.Button)(target));
            
            #line 151 "..\..\..\..\View\PointList.xaml"
            this.ChangeButton.Click += new System.Windows.RoutedEventHandler(this.ChangeButton_Click);
            
            #line default
            #line hidden
            return;
            case 14:
            this.CancelButton = ((System.Windows.Controls.Button)(target));
            
            #line 152 "..\..\..\..\View\PointList.xaml"
            this.CancelButton.Click += new System.Windows.RoutedEventHandler(this.CancelButton_Click);
            
            #line default
            #line hidden
            return;
            case 15:
            this.ConfirmButton = ((System.Windows.Controls.Button)(target));
            
            #line 153 "..\..\..\..\View\PointList.xaml"
            this.ConfirmButton.Click += new System.Windows.RoutedEventHandler(this.ConfirmButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.4.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            System.Windows.EventSetter eventSetter;
            switch (connectionId)
            {
            case 7:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.Controls.Control.MouseDoubleClickEvent;
            
            #line 77 "..\..\..\..\View\PointList.xaml"
            eventSetter.Handler = new System.Windows.Input.MouseButtonEventHandler(this.OnListViewItemDoubleClick);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            }
        }
    }
}

