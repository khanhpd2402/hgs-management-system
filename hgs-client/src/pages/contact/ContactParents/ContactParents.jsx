import React, { useState, useEffect } from 'react';
import { Select, Table, Input, Space } from 'antd';
import axios from 'axios';

const { Search } = Input;

const ContactParents = () => {
  const [academicYears, setAcademicYears] = useState([]);
  const [students, setStudents] = useState([]);
  const [selectedYear, setSelectedYear] = useState(null);
  const [searchText, setSearchText] = useState('');
  const [selectedClass, setSelectedClass] = useState(null);
  const [filteredStudents, setFilteredStudents] = useState([]);

  // Hàm chuẩn hóa chuỗi tiếng Việt
  const normalizeString = (str) => {
    return str
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/đ/g, 'd')
      .replace(/Đ/g, 'D');
  };

  // Hàm kiểm tra xem chuỗi có match với search text hay không
  const isMatchingSearch = (text, searchTerm) => {
    if (!text || !searchTerm) return false;
    
    const normalizedText = normalizeString(text);
    const normalizedSearch = normalizeString(searchTerm);
    
    // Tách từ khóa tìm kiếm thành các từ riêng biệt
    const searchWords = normalizedSearch.split(/\s+/).filter(word => word.length > 0);
    
    // Kiểm tra từng từ
    return searchWords.every(word => {
      // Kiểm tra xuôi
      if (normalizedText.includes(word)) return true;
      
      // Kiểm tra ngược
      const reversedWord = word.split('').reverse().join('');
      return normalizedText.includes(reversedWord);
    });
  };

  // Lấy danh sách năm học
  useEffect(() => {
    const fetchAcademicYears = async () => {
      try {
        const response = await axios.get('https://localhost:8386/api/AcademicYear');
        setAcademicYears(response.data);
      } catch (error) {
        console.error('Lỗi khi lấy danh sách năm học:', error);
      }
    };
    fetchAcademicYears();
  }, []);

  // Lấy danh sách học sinh khi chọn năm học
  const handleYearChange = async (value) => {
    setSelectedYear(value);
    setSelectedClass(null); // Reset lớp đã chọn
    try {
      const response = await axios.get(`https://localhost:8386/api/Student/${value}`);
      setStudents(response.data.students);
      setFilteredStudents(response.data.students);
    } catch (error) {
      console.error('Lỗi khi lấy danh sách học sinh:', error);
    }
  };

  // Lấy danh sách lớp duy nhất từ dữ liệu học sinh
  const getUniqueClasses = () => {
    const classes = [...new Set(students.map(student => student.className))];
    return classes.map(className => ({
      value: className,
      label: className
    }));
  };

  // Xử lý tìm kiếm và lọc với Fulltext Search
  useEffect(() => {
    let result = [...students];
    
    // Lọc theo lớp
    if (selectedClass) {
      result = result.filter(student => student.className === selectedClass);
    }
    
    // Lọc theo từ khóa tìm kiếm với Fulltext
    if (searchText) {
      result = result.filter(student => {
        // Tìm trong tên học sinh
        const nameMatch = isMatchingSearch(student.fullName, searchText);
        
        // Tìm trong số điện thoại
        const fatherPhoneMatch = isMatchingSearch(student.parent.phoneNumberFather, searchText);
        const motherPhoneMatch = isMatchingSearch(student.parent.phoneNumberMother, searchText);
        
        // Tìm trong tên phụ huynh
        const fatherNameMatch = isMatchingSearch(student.parent.fullNameFather, searchText);
        const motherNameMatch = isMatchingSearch(student.parent.fullNameMother, searchText);
        
        return nameMatch || fatherPhoneMatch || motherPhoneMatch || fatherNameMatch || motherNameMatch;
      });
    }
    
    setFilteredStudents(result);
  }, [selectedClass, searchText, students]);

  const columns = [
    {
      title: 'Họ và tên',
      dataIndex: 'fullName',
      key: 'fullName',
      sorter: (a, b) => normalizeString(a.fullName).localeCompare(normalizeString(b.fullName)),
    },
    {
      title: 'Lớp',
      dataIndex: 'className',
      key: 'className',
      sorter: (a, b) => a.className.localeCompare(b.className),
    },
    {
      title: 'Ngày sinh',
      dataIndex: 'dob',
      key: 'dob',
      render: (dob) => new Date(dob).toLocaleDateString('vi-VN'),
      sorter: (a, b) => new Date(a.dob) - new Date(b.dob),
    },
    {
      title: 'SĐT Bố',
      dataIndex: ['parent', 'phoneNumberFather'],
      key: 'phoneNumberFather',
    },
    {
      title: 'SĐT Mẹ',
      dataIndex: ['parent', 'phoneNumberMother'],
      key: 'phoneNumberMother',
    },
  ];

  return (
    <div className="p-4">
      <Space direction="vertical" className="w-full mb-4">
        <Space>
          <Select
            placeholder="Chọn năm học"
            style={{ width: 200 }}
            onChange={handleYearChange}
            options={academicYears.map((year) => ({
              value: year.academicYearID,
              label: year.yearName,
            }))}
          />
          
          <Select
            placeholder="Chọn lớp"
            style={{ width: 200 }}
            value={selectedClass}
            onChange={setSelectedClass}
            options={getUniqueClasses()}
            allowClear
            disabled={!selectedYear}
          />
          
          <Search
            placeholder="Tìm kiếm theo tên, SĐT hoặc tên phụ huynh"
            style={{ width: 300 }}
            onChange={(e) => setSearchText(e.target.value)}
            allowClear
          />
        </Space>
      </Space>
      
      <Table 
        columns={columns} 
        dataSource={filteredStudents}
        rowKey="studentId"
        pagination={{ pageSize: 10 }}
      />
    </div>
  );
};

export default ContactParents;
