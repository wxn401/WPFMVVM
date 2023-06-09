﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WpfCore.Helper
{
    /// <summary>
    /// 原文连接：https://www.cnblogs.com/moiska/p/5018640.html
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private Action<object> execute;                     //定义成员

        private Predicate<object> canExecute;//Predicate：述语//定义成员

        private event EventHandler CanExecuteChangedInternal;//事件

        public RelayCommand(Action<object> execute)       //定义Action，CanExecute
            : this(execute, DefaultCanExecute)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)//定义
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged        //CanExecuteChanged事件处理方法
        {
            add
            {
                CommandManager.RequerySuggested += value;
                this.CanExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                this.CanExecuteChangedInternal -= value;
            }
        }

        public bool CanExecute(object parameter)            //CanExecute方法
        {
            return this.canExecute != null && this.canExecute(parameter);
        }

        public void Execute(object parameter)              //Execute方法
        {
            this.execute(parameter);
        }

        public void OnCanExecuteChanged()                //OnCanExecute方法
        {
            EventHandler handler = this.CanExecuteChangedInternal;
            if (handler != null)
            {
                //DispatcherHelper.BeginInvokeOnUIThread(() => handler.Invoke(this, EventArgs.Empty));
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        public void Destroy()                          //销毁方法
        {
            this.canExecute = _ => false;
            this.execute = _ => { return; };
        }

        private static bool DefaultCanExecute(object parameter)  //DefaultCanExecute方法
        {
            return true;
        }
    }
}
