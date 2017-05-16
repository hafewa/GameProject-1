﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class wnd_base
{
    public wnd_base(string name)
    {
        m_name = name;
        //evt_showfinish.AddCallback(new SharpEventCallback(OnShowfinish)); 

        WndManage.Single.OnWndDestroy += OnWndDestroy;
    }

    void OnWndDestroy(Wnd wnd)
    {
        if (wnd == m_instance)
        {
            OnLostInstance();
            m_instance = null;
        }
    }

    public void Hide(float duration)
    {
        m_isVisible = false;

        //WndManage.Single.HideWnd(m_name, duration,
        //    (wnd) =>
        //    {

        //    }
        //    , null);
    }

    public void Show(float duration)
    {
        m_isVisible = true;
        //WndManage.Single.ShowWnd(m_name, duration, false, _OnShowfinish, null);
        //CoroutineManage.Single.StartCoroutine(WndManage.Single.ShowWnd(m_name, duration, _OnShowfinish));
    }

    protected abstract void OnLostInstance();
    protected abstract void OnNewInstance();

    void _OnShowfinish(Wnd instance)
    {
        m_instance = instance;
        {

            OnNewInstance();
        }
        /*
        //当前应该处于隐藏状态
        if (!m_isVisible)
            Hide(m_lastHideDuration);
        else
         */
        OnShowfinish();
    }

    protected virtual void OnShowfinish() { }

    public bool IsVisible { get { return m_isVisible; } }
    public Wnd Instance { get { return m_instance; } }

    //IQKEvent evt_showfinish = new IQKEvent();
    //float m_lastHideDuration = 0;
    protected string m_name;
    protected bool m_isVisible = false;
    protected Wnd m_instance = null;
}
