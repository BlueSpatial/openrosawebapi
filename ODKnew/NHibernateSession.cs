using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ODKnew
{
    public class NHibernateSession
    {
        public static ISession OpenSession()
        {
            var configuration = new Configuration();
            var configurationPath = HttpContext.Current.Server.MapPath(@"~\Models\hibernate.cfg.xml");
            configuration.Configure(configurationPath);
            var bookConfigurationFile = HttpContext.Current.Server.MapPath(@"~\Models\Mapping\Book.hbm.xml");
            configuration.AddFile(bookConfigurationFile);
            ISessionFactory sessionFactory = configuration.BuildSessionFactory();
            DatabaseMetadata meta = new DatabaseMetadata((DbConnection)sessionFactory.OpenSession().Connection, new NHibernate.Dialect.MsSql2008Dialect());
            if (meta.IsTable("Book"))
            {
                Debug.WriteLine("Book ada");
             }
            else {
                Debug.WriteLine("Book tidak ada");
                new SchemaUpdate(configuration).Execute(true, true);
            }
                
            
            return sessionFactory.OpenSession();
        }
        public static void ValidateSchema(Configuration config)
        {
            new SchemaValidator(config).Validate();
        }
    }
}