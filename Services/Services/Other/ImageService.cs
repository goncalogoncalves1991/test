using Amazon;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Other
{
    public class ImageService
    {
        public class Messages
        {
            public static string DEFAULT_PERSON = "File without any content, default image for a person retrieved";
            public static string DEFAULT_AVATAR = "File without any content, default image for a avatar retrieved";
           
        }


        public enum ImageIdentity { Events, Communities,Users };
        public enum ImageType { Avatar, Album }

        private const string image_start_path = "https://s3-eu-west-1.amazonaws.com/eventcommit/";
        public const string image_default = "https://s3-eu-west-1.amazonaws.com/eventcommit/Default/images.jpg";
        public const string PERSON_IMAGE_DEFAULT = "https://s3-eu-west-1.amazonaws.com/eventcommit/Default/person.png";

        private static readonly string _awsAccessKey = ConfigurationManager.AppSettings["AWSAccessKey"];

        private static readonly string _awsSecretKey = ConfigurationManager.AppSettings["AWSSecretKey"];

        private static readonly string _bucketName = ConfigurationManager.AppSettings["Bucketname"];

        public static async Task<OperationResult<string>> SendToProvider(int id, ImageIdentity entity, Stream file, ImageType type)
        {
            return await Send(id + "", entity, file, type);
        }

        public static async Task<OperationResult<string>> SendToProvider(string id, ImageIdentity entity, Stream file, ImageType type)
        {
            return await Send(id, entity, file, type);
        }

        private static async Task<OperationResult<string>> Send(string id, ImageIdentity entity, Stream file, ImageType type)
        {
            if(file != null)
            {
                OperationResult<string> f = checkFile(file);
                if (!f.Success) return new OperationResult<string> { Success = false, Result = image_default, Message = f.Message };

                try
                {
                    IAmazonS3 client;
                    using (client = AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey, RegionEndpoint.GetBySystemName("eu-west-1")))
                    {
                        string k = null;
                        string guid = null;
                        if (type == ImageType.Album)
                        {
                            guid = Guid.NewGuid().ToString() + f.Result;

                        }

                        k = String.Format("{0}/{1}/{2}/{3}", Enum.GetName(typeof(ImageIdentity), entity), id, Enum.GetName(typeof(ImageType), type), guid == null ? "profile" : guid);
                        var request = new PutObjectRequest()
                        {
                            BucketName = _bucketName,
                            CannedACL = S3CannedACL.PublicRead,
                            Key = k,
                            InputStream = file
                        };

                        await client.PutObjectAsync(request);
                        return new OperationResult<string> { Success = true, Result = image_start_path + k, Message = k };
                    }
                }
                catch (Exception ex)
                {
                    if (entity == ImageIdentity.Users)
                        return new OperationResult<string> { Success = false, Result = PERSON_IMAGE_DEFAULT, Message = ex.Message };
                    else
                        return new OperationResult<string> { Success = false, Result = image_default, Message = ex.Message };
                }
            }else
            {
                if(entity == ImageIdentity.Users)
                    return new OperationResult<string> { Success = false, Result = PERSON_IMAGE_DEFAULT, Message = Messages.DEFAULT_PERSON };
                else
                    return new OperationResult<string> { Success = false, Result = image_default, Message = Messages.DEFAULT_AVATAR};
            }
            

        }
        public static void DeleteFolder(int id, ImageService.ImageIdentity entity)
        {

            AmazonS3Config cfg = new AmazonS3Config();
            cfg.RegionEndpoint = Amazon.RegionEndpoint.EUWest1;
            AmazonS3Client s3Client = new AmazonS3Client(_awsAccessKey, _awsSecretKey, cfg);
            S3DirectoryInfo directoyToDelete = new S3DirectoryInfo(s3Client, _bucketName, String.Format("{0}/{1}", Enum.GetName(typeof(ImageIdentity), entity),id));
            directoyToDelete.Delete(true);//true will delete recursive all folder inside
            
        }

        public static void DeleteFolder(string id, ImageService.ImageIdentity entity)
        {

            AmazonS3Config cfg = new AmazonS3Config();
            cfg.RegionEndpoint = Amazon.RegionEndpoint.EUWest1;
            AmazonS3Client s3Client = new AmazonS3Client(_awsAccessKey, _awsSecretKey, cfg);
            S3DirectoryInfo directoyToDelete = new S3DirectoryInfo(s3Client, _bucketName, String.Format("{0}/{1}", Enum.GetName(typeof(ImageIdentity), entity), id));
            directoyToDelete.Delete(true);//true will delete recursive all folder inside

        }


        public static async Task<OperationResult<string>> DeleteFromProvider(string path)
        {
            
            try
            {
                IAmazonS3 client;
                using (client = AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey, RegionEndpoint.GetBySystemName("eu-west-1")))
                {
                    var request = new DeleteObjectRequest()
                    {
                        BucketName = _bucketName,
                        Key = path
                      
                    };

                    await client.DeleteObjectAsync(request);
                    return new OperationResult<string> { Success = true, Result = image_start_path };
                }
            }
            catch (Exception ex)
            {
                return new OperationResult<string> { Success = false, Result = image_default, Message = ex.Message };
            }
        }

        /**
         * if success return the extension of the file
         * else return the message of the error
             */
        private static OperationResult<string> checkFile(Stream file)
        {
            try
            {
                using (Image img = Image.FromStream(file))
                {
                    if (ImageFormat.Jpeg.Equals(img.RawFormat))
                    {
                        return new OperationResult<string>() { Success = true, Result = ".jpeg" };
                    }
                    else if (ImageFormat.Png.Equals(img.RawFormat))
                    {
                        return new OperationResult<string>() { Success = true, Result = ".png" };
                    }
                    return new OperationResult<string>() { Success = false, Result = "File must be of type .jpeg or .png" };
                }
            }
            catch (Exception e)
            {
                return new OperationResult<string> { Success = false, Result = image_default, Message = "File must be of type .jpeg or .png   Detail--> " + e.Message };
            }
        }


      
    }
}