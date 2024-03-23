﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CorePush.Firebase;
//using Microsoft.Extensions.Options;
//using System;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;
//using static GPS_Tracker.Models.Push_Notification.GoogleNotification;
//using System.Runtime;

//namespace GPS_Tracker.Models.Push_Notification
//{
//    public interface INotificationService
//    {
//        Task<ResponseModel> SendNotification(NotificationModel notificationModel);
//    }

//    public class NotificationService : INotificationService
//    {
//        private readonly FcmNotificationSetting _fcmNotificationSetting;
//        public NotificationService(IOptions<FcmNotificationSetting> settings)
//        {
//            _fcmNotificationSetting = settings.Value;
//        }

//        public async Task<ResponseModel> SendNotification(NotificationModel notificationModel)
//        {
//            ResponseModel response = new ResponseModel();
//            try
//            {
//                if (notificationModel.IsAndroiodDevice)
//                {
//                    /* FCM Sender (Android Device) */
//                    FirebaseSettings settings = new FirebaseSettings(
//                        "rasa-tracker", 
//                        "", 
//                        "reza.yy@gmail.com", 
//                        "")
//                    ;
//                    HttpClient httpClient = new HttpClient();

//                    string authorizationKey = string.Format("keyy={0}", settings.ServerKey);
//                    string deviceToken = notificationModel.DeviceId;

//                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
//                    httpClient.DefaultRequestHeaders.Accept
//                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                    DataPayload dataPayload = new DataPayload();
//                    dataPayload.Title = notificationModel.Title;
//                    dataPayload.Body = notificationModel.Body;

//                    GoogleNotification notification = new GoogleNotification();
//                    notification.Data = dataPayload;
//                    notification.Notification = dataPayload;

//                    var fcm = new FirebaseSender(settings, httpClient);
//                    var fcmSendResponse = await fcm.SendAsync(deviceToken, notification);

//                    if (fcmSendResponse.IsSuccess())
//                    {
//                        response.IsSuccess = true;
//                        response.Message = "Notification sent successfully";
//                        return response;
//                    }
//                    else
//                    {
//                        response.IsSuccess = false;
//                        response.Message = fcmSendResponse.Results[0].Error;
//                        return response;
//                    }
//                }
//                else
//                {
//                    /* Code here for APN Sender (iOS Device) */
//                    //var apn = new ApnSender(apnSettings, httpClient);
//                    //await apn.SendAsync(notification, deviceToken);
//                }
//                return response;
//            }
//            catch (Exception ex)
//            {
//                response.IsSuccess = false;
//                response.Message = "Something went wrong";
//                return response;
//            }
//        }
//    }
//}
