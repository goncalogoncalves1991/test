using DataAccess.Models.DTOs;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using DataAccess.Factories;

namespace DataAccess.Repositories
{
    public class Repository<T>
    {
        protected EventCommitEntities context;
        private const string sponsorColumName = "sponsorId";//name on the colum of type created on sql script
        private const string tagColumName = "tagId";//name on the colum of type created on sql script
        private const string sponsorTypeName = "SponsorsTable";//name  of type created on sql script
        private const string tagTypeName = "TagsTable";//name  of type created on sql script

        public Repository()
        {
            
            context = new EventCommitEntities();
            context.Configuration.LazyLoadingEnabled = false;
        }

        protected void format_tags_and_sponsors_to_parameter(int[] Tags, int[] Sponsors, out SqlParameter sponsors, out SqlParameter tags)
        {
            DataTable dtSponsor = new DataTable();
            dtSponsor.Columns.Add(sponsorColumName);
            DataTable dtTag = new DataTable();
            dtTag.Columns.Add(tagColumName);

            if (Sponsors != null)
                Sponsors.ToList().ForEach(elem =>
                {
                    DataRow drSponsor = dtSponsor.NewRow();
                    drSponsor[sponsorColumName] = elem;
                    dtSponsor.Rows.Add(drSponsor);
                });

            if (Tags != null)
                Tags.ToList().ForEach(elem =>
                {
                    DataRow drTag = dtTag.NewRow();
                    drTag[tagColumName] = elem;
                    dtTag.Rows.Add(drTag);
                });


            ////Use DbType.Structured for TVP
            sponsors = new SqlParameter("sponsors", SqlDbType.Structured);
            sponsors.Value = dtSponsor;
            sponsors.TypeName = sponsorTypeName;

            tags = new SqlParameter("tags", SqlDbType.Structured);
            tags.Value = dtTag;
            tags.TypeName = tagTypeName;
        }

        protected Task<bool> Tags_N_To_N_operations(int entityId, int[] tags, string query)
        {
            return Task.Factory.StartNew(() =>
            {

                SqlParameter sponsor;
                SqlParameter tag;
                format_tags_and_sponsors_to_parameter(tags, null, out sponsor, out tag);
                var id = SqlParameterFactory.getIntParameter("entityId", entityId);

                try
                {
                    context.Database.ExecuteSqlCommand(query, tag, id);

                    return true;
                }
                catch (SqlException ex)
                {
                    throw new ArgumentException(ex.Message);
                }
            });
        }

        protected Task<bool> Sponsors_N_To_N_operations(int communityId, int[] sponsors, string query)
        {
            return Task.Factory.StartNew(() =>
            {

                SqlParameter sponsor;
                SqlParameter tag;
                format_tags_and_sponsors_to_parameter(null, sponsors, out sponsor, out tag);
                var id = SqlParameterFactory.getIntParameter("communityId", communityId);

                try
                {
                    context.Database.ExecuteSqlCommand(query, sponsor, id);

                    return true;
                }
                catch (SqlException ex)
                {
                    throw new ArgumentException(ex.Message);
                }
            });
        }
       
       
        protected Task<int> saveInfo(T item,int id)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    context.SaveChanges();
                    return id;
                }
                catch (SqlException ex)
                {
                    throw new ArgumentException(ex.Message);
                }

            });
        }
        
      

        protected Task<string> N_To_N_operation(int entityId, string userId, string query)
        {

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var id = SqlParameterFactory.getIntParameter("id", entityId);
                    var uId = SqlParameterFactory.getVarCharParameter("userId", userId);

                    context.Database.ExecuteSqlCommand(query, id, uId);

                    return userId;
                }
                catch (SqlException ex)
                {
                    throw new ArgumentException(ex.Message);
                }

            });
        }
    }
}
