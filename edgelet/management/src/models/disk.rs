/* 
 * IoT Edge Management API
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: 2020-07-22
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */


#[allow(unused_imports)]
use serde_json::Value;

#[derive(Clone, Debug, Serialize, Deserialize)]
pub struct Disk {
  #[serde(rename = "name")]
  name: String,
  #[serde(rename = "available_space")]
  available_space: i64,
  #[serde(rename = "total_space")]
  total_space: i64,
  #[serde(rename = "file_system")]
  file_system: String,
  #[serde(rename = "file_type")]
  file_type: String
}

impl Disk {
  pub fn new(name: String, available_space: i64, total_space: i64, file_system: String, file_type: String) -> Disk {
    Disk {
      name: name,
      available_space: available_space,
      total_space: total_space,
      file_system: file_system,
      file_type: file_type
    }
  }

  pub fn set_name(&mut self, name: String) {
    self.name = name;
  }

  pub fn with_name(mut self, name: String) -> Disk {
    self.name = name;
    self
  }

  pub fn name(&self) -> &String {
    &self.name
  }


  pub fn set_available_space(&mut self, available_space: i64) {
    self.available_space = available_space;
  }

  pub fn with_available_space(mut self, available_space: i64) -> Disk {
    self.available_space = available_space;
    self
  }

  pub fn available_space(&self) -> &i64 {
    &self.available_space
  }


  pub fn set_total_space(&mut self, total_space: i64) {
    self.total_space = total_space;
  }

  pub fn with_total_space(mut self, total_space: i64) -> Disk {
    self.total_space = total_space;
    self
  }

  pub fn total_space(&self) -> &i64 {
    &self.total_space
  }


  pub fn set_file_system(&mut self, file_system: String) {
    self.file_system = file_system;
  }

  pub fn with_file_system(mut self, file_system: String) -> Disk {
    self.file_system = file_system;
    self
  }

  pub fn file_system(&self) -> &String {
    &self.file_system
  }


  pub fn set_file_type(&mut self, file_type: String) {
    self.file_type = file_type;
  }

  pub fn with_file_type(mut self, file_type: String) -> Disk {
    self.file_type = file_type;
    self
  }

  pub fn file_type(&self) -> &String {
    &self.file_type
  }


}


