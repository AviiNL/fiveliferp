using Nequeo.Linq.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiveLife.Database.SqLite
{
    public static class Repository<T> where T : class, Shared.Entity.IEntity
    {

        public static IEnumerable<T> GetAll(bool refresh = false)
        {
            var list = Connection.context.Set<T>().ToList();

            if (refresh)
            {
                foreach (var obj in list)
                {
                    Connection.context.Entry<T>(obj).Reload();
                }
            }

            return list;
        }

        public static IEnumerable<T> Find(Func<T, bool> predicate, bool refresh = false)
        {
            var list = Connection.context.Set<T>().ToList().Where(predicate);

            if (refresh)
                foreach (var obj in list)
                    Connection.context.Entry<T>(obj).Reload();

            return list;
        }

        public static IEnumerable<T> Find(string predicate, bool refresh = false)
        {
            Console.WriteLine(predicate);
            Console.WriteLine($"Refresh? {refresh.ToString()}");
            var list = Connection.context.Set<T>().Where(predicate);

            if (refresh)
                foreach (var obj in list)
                    Connection.context.Entry<T>(obj).Reload();

            return list;
        }

        public static T FindOne(Func<T, bool> predicate, bool refresh = false)
        {
            return Find(predicate, refresh).FirstOrDefault();
        }

        public static T FindOne(string predicate, bool refresh = false)
        {
            return Find(predicate, refresh).FirstOrDefault();
        }

        public static T GetById(int Id, bool refresh = false)
        {
            return Find(b => b.Id == Id, refresh).FirstOrDefault();
        }

        public static void Insert(T obj)
        {
            Connection.context.Set<T>().Add(obj);
            Connection.context.SaveChanges();
        }

        public static void Update(T obj)
        {
            var current = GetById(obj.Id);

            Connection.context.Entry(current).CurrentValues.SetValues(obj);
            Connection.context.Entry(current).State = System.Data.Entity.EntityState.Modified;

            Connection.context.SaveChanges();
        }

        public static void Delete(int Id)
        {
            var obj = Connection.context.Set<T>().Find(Id);
            Delete(obj);
        }

        public static void Delete(T obj)
        {
            Connection.context.Set<T>().Remove(obj);
            Connection.context.SaveChanges();
        }
    }
}
