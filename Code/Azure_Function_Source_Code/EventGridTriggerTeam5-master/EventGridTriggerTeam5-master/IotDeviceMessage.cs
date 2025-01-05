using System.Text.Json.Serialization;

namespace EventGridTrigger
{
    internal class IotDeviceMessage
    {
        /*
        userId: this.user.userId,
          homeId: homeId,
          roomId: roomId,
          deviceId: deviceId */

        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("homeId")]
        public string HomeId { get; set; }

        [JsonPropertyName("roomId")]
        public string RoomId { get; set; }

        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; }

    }
}
