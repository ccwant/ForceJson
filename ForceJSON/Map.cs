using System;
using System.Collections.Generic;
using System.Text;

namespace com.force.json
{
    /// <summary>
    /// Map键值对
    /// </summary>
    /// <typeparam name="TKey">键</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    public class Map<TKey, TValue> : Dictionary<TKey, TValue>
    {

        public Map(): base()
        {

        }
        /// <summary>
        /// 将指定的键和值添加到Map中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Put(TKey key, TValue value)
        {
           base.Add(key, value);
        }
        /// <summary>
        /// 根据键获取Map中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Get(TKey key)
        {
            if (ContainsKey(key))
            {
                return base[key];
            }
            return default(TValue);
        }
        /// <summary>
        /// 根据键移除Map中的键/值
        /// </summary>
        /// <param name="key"></param>
        public new void Remove(TKey key)
        {
            base.Remove(key);
        }
        /// <summary>
        /// 返回Map中的键/值的大小
        /// </summary>
        /// <returns></returns>
        public int GetSize()
        {
            return base.Count;
        }
        /// <summary>
        /// 获取键的集合
        /// </summary>
        /// <returns></returns>
        public List<TKey> GetKeys()
        {
             return new List<TKey>(base.Keys);
        }
        /// <summary>
        /// 获取值的集合
        /// </summary>
        /// <returns></returns>
        public List<TValue> GetValues()
        {
            return new List<TValue>(base.Values);
        }
    }
}
