using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ŀ���б�
/// </summary>
/// <typeparam name="T"></typeparam>
public class TargetList<T> where T : IGraphicsHolder//IGraphical<Rectangle>
{
    /// <summary>
    /// ����ȫ�����б�
    /// </summary>
    public IList<T> List { get { return list; } } 

    /// <summary>
    /// �����Ĳ����б�
    /// </summary>
    public QuadTree<T> QuadTree { get { return quadTree; } }

    /// <summary>
    /// ��ͼ��Ϣ
    /// </summary>
    public MapInfo<T> MapInfo
    {
        get { return mapinfo; }
        set { mapinfo = value; }
    }


    /// <summary>
    /// Ŀ�����б�
    /// </summary>
    private IList<T> list = null;

    /// <summary>
    /// �Ĳ���
    /// </summary>
    private QuadTree<T> quadTree = null;

    /// <summary>
    /// ��ͼ��Ϣ
    /// </summary>
    private MapInfo<T> mapinfo = null;

    ///// <summary>
    ///// ɾ���б�
    ///// </summary>
    //private IList<T> removeList = new List<T>(); 

    /// <summary>
    /// ��λ���ӿ���
    /// </summary>
    private int unitWidht;


    /// <summary>
    /// ����Ŀ���б�
    /// </summary>
    /// <param name="x">��ͼλ��x</param>
    /// <param name="y">��ͼλ��y</param>
    /// <param name="width">��ͼ����</param>
    /// <param name="height">��ͼ�߶�</param>
    /// <param name="unitWidht"></param>
    public TargetList(float x, float y, int width, int height, int unitWidht)
    {
        var mapRect = new RectGraphics(new Vector2(x, y), width * unitWidht, height * unitWidht, 0);
        this.unitWidht = unitWidht;
        quadTree = new QuadTree<T>(0, mapRect);
        list = new List<T>();
    }

    /// <summary>
    /// ���ӵ�Ԫ
    /// </summary>
    /// <param name="t">��Ԫ����, ����T</param>
    public void Add(T t)
    {
        // �ն��󲻼������
        if (t == null)
        {
            return;
        }
        // ����ȫ���б�
        list.Add(t);
        // �����Ĳ���
        quadTree.Insert(t);
    }

    /// <summary>
    /// ɾ����λ
    /// </summary>
    /// <param name="t"></param>
    public void Remove(T t)
    {
        // �ն��󲻼������
        if (t == null)
        {
            return;
        }
        list.Remove(t);
        RebuildQuadTree();
    }


    /// <summary>
    /// ���ݷ�Χ��ȡ����
    /// </summary>
    /// <param name="rect">���ζ���, �����ж���ײ</param>
    /// <returns></returns>
    public IList<T> GetListWithRectangle(RectGraphics rect)
    {
        // ���ط�Χ�ڵĶ����б�
        return quadTree.Retrieve(rect);
    }

    /// <summary>
    /// ���¹����Ĳ���
    /// ʹ�����: �б��ж���λ���ѱ��ʱ
    /// </summary>
    public void RebuildQuadTree()
    {
        // TODO û��λ�Ƶĵ�λ���������²���
        quadTree.Clear();
        quadTree.Insert(list);
    }


    public void RebulidMapInfo()
    {
        if (mapinfo != null)
        {
            mapinfo.RebuildMapInfo(list);
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void Clear()
    {
        list.Clear();
        quadTree.Clear();
        if (mapinfo != null)
        {
            mapinfo = null;
        }
    }

}