import requests
from google.auth.transport.requests import Request
from google.oauth2 import service_account

class GoogleStorageUploader:
    def __init__(self, service_account_file, bucket_name):
        self.service_account_file = service_account_file
        self.bucket_name = bucket_name
        self.access_token = self.get_access_token()

    def get_access_token(self):
        credentials = service_account.Credentials.from_service_account_file(
            self.service_account_file, scopes=["https://www.googleapis.com/auth/devstorage.full_control"]
        )
        credentials.refresh(Request())
        return credentials.token

    def upload_file(self, file_path, destination_blob_name):
        url = f"https://storage.googleapis.com/upload/storage/v1/b/{self.bucket_name}/o"
        headers = {"Authorization": f"Bearer {self.access_token}"}
        params = {"uploadType": "media", "name": destination_blob_name}

        with open(file_path, "rb") as file_data:
            response = requests.post(url, headers=headers, params=params, data=file_data)

        if response.status_code == 200:
            print("Upload successful!")
            print(response.json())
        else:
            print(f"Upload failed! Status: {response.status_code}, Response: {response.text}")

if __name__ == "__main__":
    SERVICE_ACCOUNT_FILE = "path/to/your-service-account.json"
    BUCKET_NAME = "your-bucket-name"
    FILE_PATH = "path/to/your/file.txt"
    DESTINATION_BLOB_NAME = "uploaded-file.txt"

    uploader = GoogleStorageUploader(SERVICE_ACCOUNT_FILE, BUCKET_NAME)
    uploader.upload_file(FILE_PATH, DESTINATION_BLOB_NAME)
