// import React, { useState } from 'react';
// import { Card, Table, Form, Select, Button, message } from 'antd';
// import dayjs from 'dayjs';

// const { Option } = Select;

// const SubstituteTeacherAssignment = ({ leaveRequest }) => {
//   const [loading, setLoading] = useState(false);
//   const [form] = Form.useForm();

//   // Dữ liệu mẫu cho lịch dạy cần thay thế
//   const mockSchedules = [
//     {
//       scheduleId: 1,
//       date: '2024-03-20',
//       period: 'Tiết 1-2',
//       className: '9B',
//       subject: 'Toán học',
//     },
//     {
//       scheduleId: 2,
//       date: '2024-03-20',
//       period: 'Tiết 3-4',
//       className: '9C',
//       subject: 'Toán học',
//     },
//     {
//       scheduleId: 3,
//       date: '2024-03-21',
//       period: 'Tiết 1-2',
//       className: '9A',
//       subject: 'Toán học',
//     },
//     {
//       scheduleId: 4,
//       date: '2024-03-21',
//       period: 'Tiết 5-6',
//       className: '9A',
//       subject: 'Toán học',
//     }
//   ];

//   // Dữ liệu mẫu cho giáo viên có thể dạy thay
//   const mockTeachers = [
//     { id: 'T001', name: 'Nguyễn Văn A' },
//     { id: 'T002', name: 'Trần Thị B' },
//     { id: 'T003', name: 'Lê Văn C' },
//     { id: 'T004', name: 'Phạm Thị D' }
//   ];

//   const handleAssignSubstitute = async (values) => {
//     try {
//       setLoading(true);
//       // Giả lập delay API
//       await new Promise(resolve => setTimeout(resolve, 1000));

//       console.log('Dữ liệu phân công:', values);
//       message.success('Phân công giáo viên dạy thay thành công');
//       form.resetFields(['assignments']);
//     } catch (error) {
//       message.error('Phân công giáo viên dạy thay thất bại');
//     } finally {
//       setLoading(false);
//     }
//   };

//   const columns = [
//     {
//       title: 'Ngày',
//       dataIndex: 'date',
//       key: 'date',
//       render: (text) => dayjs(text).format('DD/MM/YYYY')
//     },
//     {
//       title: 'Tiết học',
//       dataIndex: 'period',
//       key: 'period'
//     },
//     {
//       title: 'Lớp',
//       dataIndex: 'className',
//       key: 'className'
//     },
//     {
//       title: 'Môn học',
//       dataIndex: 'subject',
//       key: 'subject'
//     },

//     {
//       title: 'Giáo viên dạy thay',
//       key: 'substituteTeacher',
//       render: (_, record) => (
//         <Form.Item
//           name={['assignments', record.scheduleId]}
//           style={{ margin: 0 }}
//           rules={[{ required: true, message: 'Vui lòng chọn giáo viên!' }]}
//         >
//           <Select
//             placeholder="Chọn giáo viên dạy thay"
//             style={{ width: '100%' }}
//           >
//             {mockTeachers.map(teacher => (
//               <Option key={teacher.id} value={teacher.id}>
//                 {teacher.name}
//               </Option>
//             ))}
//           </Select>
//         </Form.Item>
//       )
//     }
//   ];

//   return (
//     <div>
//       <Card title="Thông tin lịch dạy cần thay thế" style={{ marginBottom: 16 }}>
//         <p><strong>Giáo viên nghỉ:</strong> {leaveRequest?.teacherId}</p>
//         <p><strong>Thời gian nghỉ:</strong> {dayjs(leaveRequest?.leaveFromDate).format('DD/MM/YYYY')} - {dayjs(leaveRequest?.leaveToDate).format('DD/MM/YYYY')}</p>
//       </Card>

//       <Card title="Phân công giáo viên dạy thay">
//         <Form form={form} onFinish={handleAssignSubstitute}>
//           <Table
//             dataSource={mockSchedules}
//             columns={columns}
//             rowKey="scheduleId"
//             pagination={false}
//           />
//           <Form.Item style={{ marginTop: 16, textAlign: 'right' }}>
//             <Button type="primary" htmlType="submit" loading={loading}>
//               Lưu phân công
//             </Button>
//           </Form.Item>
//         </Form>
//       </Card>
//     </div>
//   );
// };

// export default SubstituteTeacherAssignment;
