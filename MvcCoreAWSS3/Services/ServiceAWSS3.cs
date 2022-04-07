using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCoreAWSS3.Services
{
    public class ServiceAWSS3
    {
        //NECESITAMOS EL NOMBRE DEL BUCKET, PERO EN LUGAR DE ENVIAR UN 
        //STRING, VAMOS A REALIZARLO MAS PROFESIONAL
        private string bucketName;
        private IAmazonS3 awsClient;
        public ServiceAWSS3(IAmazonS3 client,
            IConfiguration configuration)
        {
            this.awsClient = client;
            this.bucketName = configuration.GetValue<string>("AWS:BucketName");
        }

        //COMENZAMOS SUBIENDO FILES AL BUCKET
        //LO QUE NOS VA A PEDIR ES EL NOMBRE DEL BUCKET, STREAM
        //Y UNA KEY/FILENAME
        //UNA VEZ SUBIDO, NOS DEVUELVE UNA RESPUESTA 200
        public async Task<bool> UploadFileAsync
            (Stream stream, string fileName)
        {
            PutObjectRequest request = new PutObjectRequest
            {
                InputStream = stream,
                Key = fileName,
                BucketName = this.bucketName
            };
            //DEBEMOS CAPTURAR UNA RESPUESTA MEDIANTE EL CLIENTE AWS
            //ENVIANDO EL REQUEST
            PutObjectResponse response =
                await this.awsClient.PutObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //VAMOS A DEVOLVER EL FILENAME SOLAMENTE
        //PARA LUEGO LEER EL ARCHIVO POR CODIGO
        public async Task<List<string>> GetFilesAsync()
        {
            //DEVUELVE UNA COLECCION LLAMADA LIST VERSIONS
            //DONDE ACCEDEMOS A LAS VERSIONES DE LOS ARCHIVOS
            ListVersionsResponse response =
                await this.awsClient.ListVersionsAsync(this.bucketName);
            return response.Versions.Select(x => x.Key).ToList();
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            DeleteObjectResponse response =
                await this.awsClient.DeleteObjectAsync
                (this.bucketName, fileName);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Stream> GetFileAsync(string fileName)
        {
            GetObjectResponse response =
                await this.awsClient.GetObjectAsync(this.bucketName, fileName);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ResponseStream;
            }
            else
            {
                return null;
            }
        }
    }
}
