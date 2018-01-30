﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Retyped.dom;

namespace System.Windows.Forms
{
    /// <summary>
    /// TODO - add controls via html....
    /// </summary>
    public class ControlCollection : IList<Control>, ICollection, IEnumerable
    {
        internal Control _owner;

        public ControlCollection(Control owner)
        {
            _owner = owner;
            _controls = new List<Control>();
        }

        public Control Owner { get { return _owner; } }

        private List<Control> _controls;

        public Control this[int index] { get { return _controls[index];  } set {
                _controls[index] = value;
            } }

        public int Count { get { return _controls.Count;  } }

        public bool IsReadOnly { get { return false; } }

        public void Add(Control item)
        {
            _owner.Element.appendChild(item.Element);
            item._parent = Owner;
            item.Load();
            _controls.Add(item);
        }

        public void AddRange(Control[] item)
        {
            if (item == null || item.Length == 0)
                return;
            var frag = document.createDocumentFragment();
            for (int i = 0; i < item.Length; i++)
            {
                frag.appendChild(item[i].Element);
                item[i]._parent = Owner;
                item[i].Load();
                _controls.Add(item[i]);                
            }
            _owner.Element.appendChild(frag);
        }

        public void Clear()
        {
            /*@
			var len = _owner.Element.childNodes.length;
			while(len--)
			{
				_owner.Element.removeChild(_owner.Element.lastChild);
			};
			*/
            _controls.Clear();
        }

        public bool Contains(Control item)
        {
            return _controls.Contains(item);
        }

        public void CopyTo(Control[] array, int arrayIndex)
        {
            _controls.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            _controls.CopyTo((Control[])array, arrayIndex);
        }

        public IEnumerator<Control> GetEnumerator()
        {
            return _controls.GetEnumerator();
        }

        public int IndexOf(Control item)
        {
            return _controls.IndexOf(item);
        }

        public void Insert(int index, Control item)
        {
            _owner.Element.insertBefore(item.Element, _owner.Element.childNodes[index]);
            _controls.Insert(index, item);
        }

        public bool Remove(Control item)
        {
            _owner.Element.removeChild(item.Element);
            return _controls.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _owner.Element.removeChild(_owner.Element.childNodes[index]);
            _controls.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _controls.GetEnumerator();
        }
    }
}
