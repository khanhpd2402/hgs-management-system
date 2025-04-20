import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Card, Descriptions, Button, message, Steps, Form, Input, Select, Spin, Alert } from 'antd';
import axios from 'axios';
import dayjs from 'dayjs';
import SubstituteTeacherAssignment from './SubstituteTeacherAssignment';

const { Option } = Select;

const LeaveRequestDetail = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [leaveRequest, setLeaveRequest] = useState(null);
  const [loading, setLoading] = useState(false);
  const [loadingTeachers, setLoadingTeachers] = useState(false);
  const [currentStep, setCurrentStep] = useState(0);
  const [form] = Form.useForm();
  const [teachers, setTeachers] = useState([]);

  useEffect(() => {
    fetchLeaveRequestDetail();
    fetchTeachers();
  }, [id]);

  useEffect(() => {
    if (leaveRequest) {
      if (leaveRequest.status === 'Approved') {
        // Nếu đã approved, có thể xem chi tiết (0) hoặc phân công (2)
        // Mặc định nên ở bước chi tiết khi mới vào trang
        // setCurrentStep(2); // Hoặc để người dùng tự chuyển
      } else if (leaveRequest.status === 'Pending') {
         // Nếu pending, có thể xem chi tiết (0) hoặc cập nhật (1)
         // Mặc định ở bước chi tiết
      } else { // Rejected
         // Nếu rejected, chỉ có thể xem chi tiết (0)
         setCurrentStep(0);
      }
    }
  }, [leaveRequest]);

  const fetchTeachers = async () => {
    setLoadingTeachers(true);
    const token = localStorage.getItem('token');

    if (!token) {
      console.error("Không tìm thấy token trong localStorage.");
      setTeachers([]);
      setLoadingTeachers(false);
      return;
    }

    const cleanedToken = token.replace(/^"|"$/g, '');
    console.log("Cleaned Token:", cleanedToken);

    try {
      console.log("Đang gọi API để lấy danh sách giáo viên...");
      const response = await axios.get('https://localhost:8386/api/Teachers', {
        headers: {
          'accept': '*/*',
          'Authorization': `Bearer ${cleanedToken}`
        }
      });

      console.log("API Response Data:", response.data);

      if (response.data && response.data.teachers && Array.isArray(response.data.teachers)) {
        const formattedTeachers = response.data.teachers.map(teacher => ({
          teacherId: teacher.teacherId || teacher.id,
          fullName: teacher.fullName || `${teacher.firstName || ''} ${teacher.lastName || ''}`.trim(),
          dob: teacher.dob
        })).filter(teacher => teacher.teacherId && teacher.fullName);

        console.log("Formatted Teachers Data:", formattedTeachers);

        setTeachers(formattedTeachers);
        console.log("Đã cập nhật state teachers.");
      } else {
         console.error("Dữ liệu API không hợp lệ hoặc không chứa mảng 'teachers':", response.data);
         setTeachers([]);
      }
    } catch (error) {
      console.error("Lỗi nghiêm trọng khi tải danh sách giáo viên:");
      if (error.response) {
        console.error("Status:", error.response.status);
        console.error("Data:", error.response.data);
        console.error("Headers:", error.response.headers);
      } else if (error.request) {
        console.error("Request Error: Không nhận được phản hồi từ server.", error.request);
      } else {
        console.error("Error setting up request:", error.message);
      }
      console.error("Full Error Object:", error.config);
      setTeachers([]);
    } finally {
      setLoadingTeachers(false);
    }
  };

  const fetchLeaveRequestDetail = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
      if (!token) {
          message.error('Token không hợp lệ hoặc không tìm thấy.');
          setLoading(false);
          setLeaveRequest(null); // Đảm bảo không có dữ liệu cũ hiển thị
          return;
      }
      const response = await axios.get(`https://localhost:8386/api/LeaveRequest/${id}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'accept': '*/*'
        }
      });
      setLeaveRequest(response.data);
    } catch (error) {
      console.error('Lỗi khi tải chi tiết yêu cầu nghỉ phép:', error);
      const errorMsg = error.response?.data?.message || error.message || 'Không thể tải chi tiết yêu cầu nghỉ phép';
      message.error(errorMsg);
      setLeaveRequest(null); // Đặt là null khi lỗi
    } finally {
      setLoading(false);
    }
  };

  const getTeacherDisplayInfo = (teacherId) => {
    if (loadingTeachers) {
        return <Spin size="small" />;
    }
    const teacher = teachers.find(t => t.teacherId === teacherId);
    if (teacher) {
        return (
            <>
                <div><strong>{teacher.fullName}</strong> (ID: {teacherId})</div>
                <div style={{ fontSize: '12px', color: '#666' }}>
                    Ngày sinh: {teacher.dob ? dayjs(teacher.dob).format('DD/MM/YYYY') : 'N/A'}
                </div>
            </>
        );
    }
    // Nếu không tìm thấy giáo viên trong danh sách đã tải
    return `ID: ${teacherId} (Không tìm thấy thông tin chi tiết)`;
  };

  const handleUpdateStatus = async (values) => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
       if (!token) {
          message.error('Token không hợp lệ hoặc không tìm thấy.');
          setLoading(false);
          return;
       }

      const updatedRequest = {
        reason: values.reason,
        status: values.status
      };

      await axios.put(`https://localhost:8386/api/LeaveRequest/${id}`, updatedRequest, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      message.success(`Yêu cầu đã được ${values.status === 'Approved' ? 'phê duyệt' : 'từ chối'} thành công`);

      await fetchLeaveRequestDetail();

      if (values.status === 'Approved') {
         setCurrentStep(2);
      } else {
         setCurrentStep(0);
      }
    } catch (error) {
      console.error('Lỗi khi cập nhật yêu cầu nghỉ phép:', error);
       const errorMsg = error.response?.data?.message || error.message || 'Cập nhật trạng thái thất bại';
      message.error(errorMsg);
    } finally {
      setLoading(false);
    }
  };

  const steps = [
    {
      title: 'Xem chi tiết',
      content: (
        <Card loading={loading && !leaveRequest}>
           {leaveRequest && (
             <Descriptions bordered column={1}>
                <Descriptions.Item label="ID Yêu cầu">{leaveRequest.requestId}</Descriptions.Item>
                <Descriptions.Item label="Giáo viên">
                    {getTeacherDisplayInfo(leaveRequest.teacherId)}
                </Descriptions.Item>
                <Descriptions.Item label="Ngày yêu cầu">
                  {leaveRequest.requestDate ? dayjs(leaveRequest.requestDate).format('DD/MM/YYYY') : ''}
                </Descriptions.Item>
                <Descriptions.Item label="Ngày bắt đầu nghỉ">
                  {leaveRequest.leaveFromDate ? dayjs(leaveRequest.leaveFromDate).format('DD/MM/YYYY') : ''}
                </Descriptions.Item>
                <Descriptions.Item label="Ngày kết thúc nghỉ">
                  {leaveRequest.leaveToDate ? dayjs(leaveRequest.leaveToDate).format('DD/MM/YYYY') : ''}
                </Descriptions.Item>
                <Descriptions.Item label="Lý do">{leaveRequest.reason}</Descriptions.Item>
                <Descriptions.Item label="Trạng thái">
                  <span style={{
                    color: leaveRequest.status === 'Pending' ? 'orange'
                         : leaveRequest.status === 'Approved' ? 'green'
                         : 'red',
                    fontWeight: 'bold'
                  }}>
                    {leaveRequest.status === 'Pending' ? 'Đang chờ duyệt'
                     : leaveRequest.status === 'Approved' ? 'Đã phê duyệt'
                     : 'Đã từ chối'}
                  </span>
                </Descriptions.Item>
             </Descriptions>
           )}
        </Card>
      )
    },
    {
      title: 'Cập nhật trạng thái',
      disabled: leaveRequest?.status !== 'Pending',
      content: (
        <Card loading={loading}>
           {leaveRequest?.status === 'Pending' ? (
             <Form
                form={form}
                layout="vertical"
                onFinish={handleUpdateStatus}
                initialValues={{
                  status: 'Approved',
                  reason: leaveRequest?.reason
                }}
                key={leaveRequest?.requestId}
             >
                <Form.Item
                  name="status"
                  label="Hành động"
                  rules={[{ required: true, message: 'Vui lòng chọn hành động!' }]}
                >
                  <Select>
                    <Option value="Approved">Phê duyệt</Option>
                    <Option value="Rejected">Từ chối</Option>
                  </Select>
                </Form.Item>

                <Form.Item
                  name="reason"
                  label="Lý do / Ghi chú cập nhật"
                  rules={[{ required: true, message: 'Vui lòng nhập lý do/ghi chú!' }]}
                >
                  <Input.TextArea rows={4} placeholder="Nhập lý do phê duyệt hoặc từ chối..." />
                </Form.Item>

                <Form.Item>
                   <Button type="primary" htmlType="submit" loading={loading}>
                      Xác nhận
                   </Button>
                </Form.Item>
             </Form>
           ) : (
              <Alert
                 message={`Không thể cập nhật yêu cầu này.`}
                 description={`Yêu cầu đang ở trạng thái "${leaveRequest?.status}". Chỉ có thể cập nhật yêu cầu đang ở trạng thái "Pending".`}
                 type="warning"
                 showIcon
              />
           )}
        </Card>
      )
    },
    {
      title: 'Phân công dạy thay',
      disabled: leaveRequest?.status !== 'Approved',
      content: (
        <Card loading={loading}>
          {leaveRequest?.status === 'Approved' ? (
            <SubstituteTeacherAssignment leaveRequest={leaveRequest} allTeachers={teachers} />
         ) : (
             <Alert
                 message={`Chưa thể phân công dạy thay.`}
                 description={leaveRequest?.status === 'Rejected'
                    ? 'Yêu cầu đã bị từ chối.'
                    : 'Yêu cầu cần được phê duyệt trước khi phân công.'}
                 type="info"
                 showIcon
              />
          )} 
        </Card>
      )
    }
  ];

  if (loading && !leaveRequest) {
    return <div style={{ padding: '50px', textAlign: 'center' }}><Spin size="large" tip="Đang tải chi tiết yêu cầu..." /></div>;
  }
   if (!loading && !leaveRequest) {
      return <div style={{ padding: '24px' }}><Alert message="Lỗi" description="Không tìm thấy yêu cầu nghỉ phép hoặc có lỗi xảy ra khi tải dữ liệu." type="error" showIcon /></div>;
   }

  return (
    <div style={{ padding: '24px' }}>
       <h1>
         Chi tiết yêu cầu nghỉ phép
         {leaveRequest && teachers.find(t => t.teacherId === leaveRequest.teacherId)
            ? ` - ${teachers.find(t => t.teacherId === leaveRequest.teacherId).fullName}`
            : leaveRequest ? ` (ID: ${leaveRequest.teacherId})` : ''}
       </h1>

      <Steps
        current={currentStep}
        items={steps.map(item => ({ title: item.title, disabled: item.disabled }))}
        onChange={setCurrentStep}
        style={{ marginBottom: '24px' }}
      />

      <div style={{ marginTop: '24px' }}>
         {steps[currentStep].content}
      </div>

      <div style={{ marginTop: '24px', display: 'flex', justifyContent: 'space-between' }}>
         <div>

            {/* Nút Về danh sách (Back to List) - giữ nguyên */}
            <Button style={{ marginLeft: '8px' }} onClick={() => navigate('/system/leave-request')} disabled={loading}>
                Về danh sách
             </Button>
         </div>
         <div>
             {/* Nút Tiếp theo (Next Step) */}
             {currentStep < steps.length - 1 && (
                <Button
                    type="primary"
                    onClick={() => setCurrentStep(currentStep + 1)}
                    style={{ marginLeft: '8px' }}
                 >
                    Tiếp theo
                </Button>
             )}
               {/* Nút Quay lại (Previous Step) */}
            {currentStep > 0 && (
               <Button onClick={() => setCurrentStep(currentStep - 1)} disabled={loading}>
                  Quay lại
               </Button>
            )}
         </div>
      </div>
    </div>
  );
};

export default LeaveRequestDetail;
