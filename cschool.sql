--
-- Database: `cschool`
--

DROP DATABASE IF EXISTS CSchool;
CREATE DATABASE CSchool;
USE CSchool;

-- --------------------------------------------------------

--
-- Table structure for table `assign_classes`
--

CREATE TABLE `assign_classes` (
  `id` int(11) NOT NULL,
  `class_id` int(11) NOT NULL,
  `head_teacher_id` int(11) NOT NULL,
  `term_id` int(11) NOT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `assign_classes`
--

INSERT INTO `assign_classes` (`id`, `class_id`, `head_teacher_id`, `term_id`, `status`) VALUES
(11, 1, 1, 9, 1),
(12, 2, 2, 9, 1),
(13, 3, 3, 9, 1),
(14, 4, 4, 9, 1),
(15, 5, 5, 9, 1),
(16, 6, 6, 9, 1),
(17, 7, 7, 9, 1),
(18, 8, 8, 9, 1);

-- --------------------------------------------------------

--
-- Table structure for table `assign_class_students`
--

CREATE TABLE `assign_class_students` (
  `assign_class_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `role` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `assign_class_students`
--

INSERT INTO `assign_class_students` (`assign_class_id`, `student_id`, `role`) VALUES
(11, 1, 'Lớp trưởng'),
(12, 2, 'Lớp phó học tập'),
(12, 3, 'Lớp phó lao động'),
(12, 4, 'Thành viên'),
(12, 5, 'Thành viên'),
(12, 6, 'Thành viên'),
(12, 7, 'Thành viên'),
(16, 8, 'Thành viên'),
(15, 9, 'Thành viên'),
(17, 10, 'Thành viên'),

(18, 11, 'Lớp trưởng'),
(11, 14, 'Thành viên'),
(11, 15, 'Thành viên'),
(15, 16, 'Thành viên'),
(16, 17, 'Thành viên'),
(13, 18, 'Thành viên'),
(13, 19, 'Thành viên'),
(14, 20, 'Thành viên'),

(11, 21, 'Lớp trưởng'),
(16, 22, 'Lớp phó học tập'),
(13, 23, 'Lớp phó lao động'),
(14, 24, 'Thành viên'),
(15, 25, 'Thành viên'),
(16, 26, 'Thành viên'),
(17, 27, 'Thành viên'),
(18, 28, 'Thành viên'),

(11, 31, 'Lớp trưởng'),
(13, 32, 'Lớp phó học tập'),
(14, 33, 'Lớp phó lao động'),
(15, 34, 'Thành viên'),
(15, 35, 'Thành viên'),
(16, 36, 'Thành viên'),
(16, 37, 'Thành viên'),
(17, 38, 'Thành viên'),
(17, 39, 'Thành viên'),
(18, 40, 'Thành viên'),

(11, 43, 'Lớp phó lao động'),
(16, 44, 'Thành viên'),
(17, 45, 'Thành viên'),
(13, 46, 'Thành viên'),
(13, 47, 'Thành viên'),
(14, 48, 'Thành viên'),
(14, 49, 'Thành viên'),
(15, 50, 'Thành viên');

-- --------------------------------------------------------

--
-- Table structure for table `assign_class_teachers`
--

CREATE TABLE `assign_class_teachers` (
  `assign_class_id` int(11) NOT NULL,
  `teacher_id` int(11) NOT NULL,
  `subject_id` int(11) NOT NULL,
  `quiz_count` int(11) DEFAULT NULL,
  `oral_count` int(11) DEFAULT NULL,
  `day` varchar(100) NOT NULL,
  `start_period` int(11) DEFAULT NULL,
  `end_period` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `assign_class_teachers`
--

INSERT INTO `assign_class_teachers` (`assign_class_id`, `teacher_id`, `subject_id`, `quiz_count`, `oral_count`, `day`, `start_period`, `end_period`) VALUES
(11, 2, 1, 2, 2, 'Thứ Ba', 2, 5),
(11, 3, 7, 2, 2, 'Thứ Bảy', 1, 5),
(11, 1, 9, 1, 1, 'Thứ Hai', 1, 2),
(12, 1, 1, 2, 2, 'Thứ Hai', 1, 3),  -- Thêm để nhất quán với score_details (Toán học)
(12, 2, 2, 2, 2, 'Thứ Tư', 2, 4),  -- Thêm để nhất quán với score_details (Ngữ văn)
(13, 1, 4, 1, 2, 'Thứ Ba', 2, 4),
(13, 3, 5, 1, 1, 'Thứ Ba', 2, 3),
(14, 4, 1, 2, 2, 'Thứ Năm', 4, 5),
(15, 5, 6, 3, 1, 'Thứ Ba', 1, 2),
(16, 6, 9, 2, 2, 'Thứ Tư', 3, 4);

-- --------------------------------------------------------

--
-- Table structure for table `classes`
--

CREATE TABLE `classes` (
  `id` int(11) NOT NULL,
  `class_type_id` int(11) NOT NULL,
  `grade` int(11) DEFAULT NULL,
  `name` varchar(100) NOT NULL,
  `area` varchar(50) DEFAULT NULL,
  `room` varchar(50) DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `classes`
--

INSERT INTO `classes` (`id`, `class_type_id`, `grade`, `name`, `area`, `room`, `status`) VALUES
(1, 2, 10, '10A', 'Khu A', 'Phòng 101', 1),
(2, 2, 10, '10B', 'Khu A', 'Phòng 102', 1),
(3, 2, 10, '10C', 'Khu B', 'Phòng 201', 1),
(4, 3, 11, '11A', 'Khu B', 'Phòng 202', 1),
(5, 3, 11, '11B', 'Khu C', 'Phòng 301', 1),
(6, 4, 12, '12A1', 'Khu C', 'Phòng 302', 1),
(7, 4, 12, '12B', 'Khu D', 'Phòng 401', 1),
(8, 7, 12, '12A2', 'Khu D', 'Phòng 405', 1);

-- --------------------------------------------------------

--
-- Table structure for table `class_types`
--

CREATE TABLE `class_types` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `class_types`
--

INSERT INTO `class_types` (`id`, `name`, `description`, `status`) VALUES
(1, 'Chuyên anh', '', 1),
(2, 'Chuyên toán', '', 1),
(3, 'Chuyên văn', '', 1),
(4, 'Chuyên Lí', '', 1),
(5, 'Chuyên tin', '', 1),
(6, 'Chuyên hóa', '', 1),
(7, 'Không chuyên', '', 1);

-- --------------------------------------------------------

--
-- Table structure for table `departments`
--

CREATE TABLE `departments` (
  `id` int(11) NOT NULL,
  `subject_id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `departments`
--

INSERT INTO `departments` (`id`, `subject_id`, `name`, `description`, `status`) VALUES
(1, 1, 'Tổ Toán', 'Phụ trách giảng dạy và phát triển chương trình môn Toán học.', 1),
(2, 2, 'Tổ Ngữ Văn', 'Chuyên môn về giảng dạy tiếng Việt và văn học.', 1),
(3, 3, 'Tổ Tiếng Anh', 'Phụ trách đào tạo và kiểm tra năng lực tiếng Anh học sinh.', 1),
(4, 4, 'Tổ Vật Lý', 'Tổ chuyên môn nghiên cứu và giảng dạy các kiến thức vật lý.', 1),
(5, 5, 'Tổ Hóa Học', 'Phụ trách giảng dạy môn Hóa và tổ chức thí nghiệm thực hành.', 1),
(6, 6, 'Tổ Sinh Học', 'Chuyên môn về sinh học, môi trường và sức khỏe học đường.', 1),
(7, 7, 'Tổ Lịch Sử', 'Tổ chức các hoạt động học tập và ngoại khóa về lịch sử dân tộc.', 1),
(8, 8, 'Tổ Địa Lý', 'Phụ trách giảng dạy kiến thức địa lý và kỹ năng bản đồ.', 1),
(9, 9, 'Tổ Tin Học', 'Chuyên môn về công nghệ thông tin, lập trình và kỹ năng số.', 1),
(10, 10, 'Tổ GDCD', 'Giảng dạy đạo đức, pháp luật và kỹ năng sống cho học sinh.', 1);

-- --------------------------------------------------------

--
-- Table structure for table `department_details`
--

CREATE TABLE `department_details` (
  `department_id` int(11) NOT NULL,
  `teacher_id` int(11) NOT NULL,
  `start_date` date DEFAULT NULL,
  `end_date` date DEFAULT NULL,
  `role` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `department_details`
--

INSERT INTO `department_details` (`department_id`, `teacher_id`, `start_date`, `end_date`, `role`) VALUES
(1, 1, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Toán'),
(2, 2, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Ngữ Văn'),
(3, 3, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Tiếng Anh'),
(4, 4, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Vật Lý'),
(5, 5, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Hóa Học'),
(6, 6, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Sinh Học'),
(7, 7, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Lịch Sử'),
(8, 8, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Địa Lý'),
(9, 9, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Tin Học'),
(10, 10, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ GDCD');

-- --------------------------------------------------------

--
-- Table structure for table `exams`
--

CREATE TABLE `exams` (
    `id` int(11) NOT NULL,
    `exam_detail_id` int(11) NOT NULL,      -- Tham chiếu đến ca thi
    `exam_room` int(11) NOT NULL,   -- Phòng thi
    `supervisor_id` int(11) DEFAULT NULL   -- Giáo viên coi thi (nếu có)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `exams`
--

INSERT INTO `exams` (`id`, `exam_detail_id`, `exam_room`, `supervisor_id`) VALUES
(1, 1, 1, 5),
(2, 2, 2, 7),
(3, 3, 3, 8),
(4, 4, 4, 6),
(5, 5, 5, 9);

-- --------------------------------------------------------

--
-- Table structure for table `rooms`
--

CREATE TABLE `rooms` (
    `id` int(11) NOT NULL,
    `name` varchar(50) NOT NULL,
    `quantity` int(11) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `rooms`
--

INSERT INTO `rooms` (`id`, `name`, `quantity`) VALUES
(1, 'Phòng A101', 10),
(2, 'Phòng A102', 10),
(3, 'Phòng B201', 10),
(4, 'Phòng B202', 10),
(5, 'Phòng C301', 10),
(6, 'Phòng C302', 10),
(7, 'Phòng D401', 10),
(8, 'Phòng D402', 10),
(9, 'Phòng E501', 10),
(10, 'Phòng E502', 10);

-- --------------------------------------------------------

--
-- Table structure for table `exam_details`
--

CREATE TABLE `exam_details` (
    `id` int(11) NOT NULL,
    `subject_id` int(11) NOT NULL,          -- Môn học được thi
    `term_id` int(11) NOT NULL,             -- Học kỳ / năm học
    `exam_type_id` int(11) DEFAULT NULL,    -- Loại bài thi: giữa kỳ, cuối kỳ (tham chiếu exam_types)
    `start_time` datetime NOT NULL,     -- Giờ bắt đầu thi
    `end_time` datetime NOT NULL,       -- Giờ kết thúc thi
    `status` TINYINT(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `exam_details`
--

INSERT INTO `exam_details` (`id`, `subject_id`, `term_id`, `exam_type_id`, `start_time`, `end_time`) VALUES
(1, 1, 9, 3, '2025-11-01 08:00:00', '2025-11-01 09:30:00'),
(2, 2, 9, 3, '2025-11-02 08:00:00', '2025-11-02 09:30:00'),
(3, 3, 9, 4, '2025-11-03 08:00:00', '2025-11-03 09:30:00'),
(4, 4, 9, 4, '2025-11-04 08:00:00', '2025-11-04 09:30:00'),
(5, 1, 9, 3, '2025-11-05 08:00:00', '2025-11-05 09:30:00');

-- --------------------------------------------------------

--
-- Table structure for table `exam_types`
--

CREATE TABLE `exam_types` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `weight` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `exam_types`
--

INSERT INTO `exam_types` (`id`, `name`, `weight`) VALUES
(1, 'Kiểm tra miệng', 1),
(2, 'Kiểm tra 15 phút', 1),
(3, 'Thi giữa kỳ', 2),
(4, 'Thi cuối kỳ', 3);

-- --------------------------------------------------------

--
-- Table structure for table `functions`
--

CREATE TABLE `functions` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL,
  `is_teacher_function` TINYINT(1) DEFAULT 0,
  `actions` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `functions`
--

INSERT INTO `functions` (`id`, `name`, `is_teacher_function`,`actions`) VALUES
(1, 'Lớp chủ nhiệm', 1, null),
(2, 'Điểm danh', 1, null),
(3, 'Nhập điểm', 1, null),
(4, 'Thống kê', 0, 'Xem'),
(5, 'Phân công giáo viên', 0, 'Xem|Thêm|Cập nhật|Xoá / Khoá'),
(6, 'Lịch thi', 0, 'Xem|Thêm|Cập nhật|Xoá / Khoá'),
(7, 'Học phí', 0, 'Xem|Thêm|Cập nhật'),
(8, 'Lớp học', 0, 'Xem|Thêm|Cập nhật|Xoá / Khoá|Xuất Excel'),
(9, 'Giáo viên', 0, 'Xem|Thêm|Cập nhật|Xoá / Khoá|Nhập Excel|Xuất Excel'),
(10, 'Học sinh', 0, 'Xem|Thêm|Cập nhật|Xoá / Khoá|Nhập Excel|Xuất Excel'),
(11, 'Nhóm quyền', 0, 'Xem|Thêm|Cập nhật|Xoá / Khoá|Nhập Excel|Xuất Excel'),
(12, 'Người dùng', 0, 'Xem|Thêm|Cập nhật|Xoá / Khoá|Nhập Excel|Xuất Excel');

-- --------------------------------------------------------

--
-- Table structure for table `roles`
--

CREATE TABLE `roles` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL,
  `status` varchar(100) DEFAULT 'Hoạt động'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `roles`
--

INSERT INTO `roles` (`id`, `name`, `status`) VALUES
(1, 'Giáo viên trường', 'Hoạt động'),
(2, 'Quản trị viên', 'Hoạt động'),
(3, 'Quản lý đào tạo', 'Hoạt động'),
(4, 'Quản lý tài chính', 'Hoạt động'),
(5, 'Quản lý thông tin', 'Hoạt động'),
(6, 'Quản lý người dùng', 'Hoạt động');
-- --------------------------------------------------------

--
-- Table structure for table `role_details`
--

CREATE TABLE `role_details` (
  `role_id` int(11) NOT NULL,
  `function_id` int(11) NOT NULL,
  `action` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `role_details`
--

INSERT INTO `role_details` (`role_id`, `function_id`, `action`) VALUES
(1, 1, ''), (1, 2, ''), (1, 3, ''),
(2, 4, 'Xem'), (2, 5, 'Xem'), (2, 5, 'Thêm'), (2, 5, 'Cập nhật'), (2, 5, 'Xoá / Khoá'),
	(2, 6, 'Xem'), (2, 6, 'Thêm'), (2, 6, 'Cập nhật'), (2, 6, 'Xoá / Khoá'),
    (2, 7, 'Xem'), (2, 7, 'Thêm'), (2, 7, 'Cập nhật'),
    (2, 8, 'Xem'), (2, 8, 'Thêm'), (2, 8, 'Cập nhật'), (2, 8, 'Xoá / Khoá'), (2, 8, 'Xuất Excel'),
    (2, 9, 'Xem'), (2, 9, 'Thêm'), (2, 9, 'Cập nhật'), (2, 9, 'Xoá / Khoá'), (2, 9, 'Nhập Excel'), (2, 9, 'Xuất Excel'),
    (2, 10, 'Xem'), (2, 10, 'Thêm'), (2, 10, 'Cập nhật'), (2, 10, 'Xoá / Khoá'), (2, 10, 'Nhập Excel'), (2, 10, 'Xuất Excel'),
    (2, 11, 'Xem'), (2, 11, 'Thêm'), (2, 11, 'Cập nhật'), (2, 11, 'Xoá / Khoá'), (2, 11, 'Nhập Excel'), (2, 11, 'Xuất Excel'),
    (2, 12, 'Xem'), (2, 12, 'Thêm'), (2, 12, 'Cập nhật'), (2, 12, 'Xoá / Khoá'), (2, 12, 'Nhập Excel'), (2, 12, 'Xuất Excel'),
(3, 5, 'Xem'), (3, 5, 'Thêm'), (3, 5, 'Cập nhật'), (3, 5, 'Xoá / Khoá'),
	(3, 6, 'Xem'), (3, 6, 'Thêm'), (3, 6, 'Cập nhật'), (3, 6, 'Xoá / Khoá'),
(4, 7, 'Xem'), (4, 7, 'Thêm'), (4, 7, 'Cập nhật'),
(5, 8, 'Xem'), (5, 8, 'Thêm'), (5, 8, 'Cập nhật'), (5, 8, 'Xoá / Khoá'), (5, 8, 'Xuất Excel'),
    (5, 9, 'Xem'), (5, 9, 'Thêm'), (5, 9, 'Cập nhật'), (5, 9, 'Xoá / Khoá'), (5, 9, 'Nhập Excel'), (5, 9, 'Xuất Excel'),
    (5, 10, 'Xem'), (5, 10, 'Thêm'), (5, 10, 'Cập nhật'), (5, 10, 'Xoá / Khoá'), (5, 10, 'Nhập Excel'), (5, 10, 'Xuất Excel'),
(6, 11, 'Xem'), (6, 11, 'Thêm'), (6, 11, 'Cập nhật'), (6, 11, 'Xoá / Khoá'), (6, 11, 'Nhập Excel'), (6, 11, 'Xuất Excel'),
    (6, 12, 'Xem'), (6, 12, 'Thêm'), (6, 12, 'Cập nhật'), (6, 12, 'Xoá / Khoá'), (6, 12, 'Nhập Excel'), (6, 12, 'Xuất Excel'); 

-- --------------------------------------------------------

--
-- Table structure for table `rules`
--

CREATE TABLE `rules` (
  `id` int(11) NOT NULL,
  `title` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `rules`
--

INSERT INTO `rules` (`id`, `title`, `description`, `status`) VALUES
(1, 'Đi học đúng giờ', 'Học sinh phải có mặt tại lớp trước giờ học quy định.', 1),
(2, 'Mặc đồng phục', 'Học sinh phải mặc đồng phục theo quy định của nhà trường vào các ngày học.', 1),
(3, 'Giữ gìn vệ sinh', 'Không xả rác bừa bãi, giữ gìn vệ sinh lớp học và khuôn viên trường.', 1),
(4, 'Không sử dụng điện thoại', 'Không sử dụng điện thoại trong giờ học nếu không có sự cho phép của giáo viên.', 1),
(5, 'Tôn trọng thầy cô', 'Học sinh phải lễ phép, tôn trọng giáo viên và nhân viên nhà trường.', 1),
(6, 'Không gây mất trật tự', 'Không nói chuyện riêng, gây ồn ào trong lớp học và khu vực học tập.', 1),
(7, 'Tham gia đầy đủ các hoạt động', 'Học sinh phải tham gia đầy đủ các hoạt động ngoại khóa, sinh hoạt lớp và các buổi học chính khóa.', 1),
(8, 'Không hút thuốc', 'Nghiêm cấm học sinh hút thuốc trong khuôn viên trường học.', 1),
(9, 'Không đánh nhau', 'Cấm mọi hành vi gây gổ, đánh nhau trong và ngoài trường học.', 1),
(10, 'Bảo vệ tài sản chung', 'Không làm hư hỏng bàn ghế, thiết bị học tập và tài sản của nhà trường.', 1);

-- --------------------------------------------------------

--
-- Table structure for table `score_details`
--

CREATE TABLE `score_details` (
  `assign_class_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `subject_id` int(11) NOT NULL,
  `exam_type_id` int(11) NOT NULL,
  `attempt` int(11) NOT NULL,
  `score` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `score_details`
--

INSERT INTO `score_details` (`assign_class_id`, `student_id`, `subject_id`, `exam_type_id`, `attempt`, `score`) VALUES
(12, 2, 1, 1, 1, 7.00),
(12, 2, 1, 2, 1, 6.75),
(12, 2, 1, 3, 1, 9.00),
(12, 2, 1, 4, 1, 8.25),
(12, 2, 2, 1, 1, 9.50),
(12, 2, 2, 1, 2, 9.00),
(12, 2, 2, 1, 3, 8.50),
(12, 2, 2, 1, 4, 8.00),
(12, 3, 1, 1, 1, 8.50),
(12, 3, 1, 2, 1, 7.50),
(12, 3, 1, 3, 1, 7.50),
(12, 3, 1, 4, 1, 7.00),
(12, 3, 2, 1, 1, 9.50),
(12, 3, 2, 2, 1, 8.50),
(12, 3, 2, 3, 1, 7.50),
(12, 3, 2, 4, 1, 7.50),
(12, 4, 1, 1, 1, 8.50),
(12, 4, 1, 2, 1, 7.50),
(12, 4, 1, 3, 1, 7.75),
(12, 4, 1, 4, 1, 7.00),
(12, 4, 2, 1, 1, 9.75),
(12, 4, 2, 2, 1, 8.50),
(12, 4, 2, 3, 1, 7.00),
(12, 4, 2, 4, 1, 8.50),
(12, 5, 1, 1, 1, 8.50),
(12, 5, 1, 2, 1, 10.00),
(12, 5, 1, 3, 1, 7.50),
(12, 5, 1, 4, 1, 7.50),
(12, 5, 2, 1, 1, 9.50),
(12, 5, 2, 2, 1, 9.50),
(12, 5, 2, 3, 1, 7.00),
(12, 5, 2, 4, 1, 7.50),
(12, 6, 1, 1, 1, 8.50),
(12, 6, 1, 2, 1, 7.50),
(12, 6, 1, 3, 1, 9.50),
(12, 6, 1, 4, 1, 7.00),
(12, 6, 2, 1, 1, 9.50),
(12, 6, 2, 2, 1, 8.50),
(12, 6, 2, 3, 1, 7.50),
(12, 6, 2, 4, 1, 9.50),
(12, 7, 1, 1, 1, 10.00),
(12, 7, 1, 2, 1, 8.50),
(12, 7, 1, 3, 1, 7.50),
(12, 7, 1, 4, 1, 6.00),
(12, 7, 2, 1, 1, 5.50),
(12, 7, 2, 2, 1, 5.50),
(12, 7, 2, 3, 1, 7.50),
(12, 7, 2, 4, 1, 9.50);

-- --------------------------------------------------------

--
-- Table structure for table `students`
--

CREATE TABLE `students` (
  `id` int(11) NOT NULL,
  `fullname` varchar(100) NOT NULL,
  `avatar` varchar(255) DEFAULT NULL,
  `birthday` date DEFAULT NULL,
  `gender` enum('Nam','Nữ','Other') DEFAULT NULL,
  `ethnicity` varchar(50) DEFAULT NULL,
  `religion` varchar(50) DEFAULT NULL,
  `address` varchar(200) DEFAULT NULL,
  `phone` varchar(10) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `learn_year` varchar(100) DEFAULT NULL,
  `learn_status` varchar(15) DEFAULT NULL,
  `parent_name` varchar(100) NOT NULL,
  `parent_phone` varchar(10) DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `students`
--

INSERT INTO `students` (`id`, `fullname`, `avatar`, `birthday`, `gender`, `ethnicity`, `religion`, `address`, `phone`, `email`, `learn_year`, `learn_status`, `parent_name`, `parent_phone`, `status`) VALUES
(1, 'Nguyễn Văn An', NULL, '2010-05-12', 'Nam', 'Kinh', 'Không', '123 Lý Thường Kiệt, TP. Hồ Chí Minh', '0912345678', 'an.nguyen@example.com', '2021-2024', 'Đang học', 'Nguyễn Văn A', '0901111111', 1),
(2, 'Trần Thị Bình', NULL, '2010-09-23', 'Nữ', 'Kinh', 'Phật giáo', '45 Nguyễn Trãi, TP. Hồ Chí Minh', '0923456789', 'binh.tran@example.com', '2022-2025', 'Đang học', 'Trần Thị M', '0902222222', 1),
(3, 'Lê Văn Cường', NULL, '2009-03-15', 'Nam', 'Mường', 'Không', '67 Hai Bà Trưng, TP. Hồ Chí Minh', '0934567890', 'cuong.le@example.com', '2020-2023', 'Bảo lưu', 'Lê Văn B', '0903333333', 1),
(4, 'Phạm Thị Dung', NULL, '2011-07-08', 'Nữ', 'Kinh', 'Công giáo', '89 Điện Biên Phủ, TP. Hồ Chí Minh', '0945678901', 'dung.pham@example.com', '2021-2024', 'Đang học', 'Phạm Thị H', '0904444444', 1),
(5, 'Hoàng Văn Em', NULL, '2011-11-30', 'Nam', 'Thái', 'Không', '12 Cách Mạng Tháng Tám, TP. Hồ Chí Minh', '0956789012', 'em.hoang@example.com', '2022-2025', 'Đang học', 'Hoàng Văn D', '0905555555', 1),
(6, 'Đỗ Thị Hạnh', NULL, '2008-04-22', 'Nữ', 'Kinh', 'Phật giáo', '234 Nguyễn Văn Cừ, TP. Hồ Chí Minh', '0967890123', 'hanh.do@example.com', '2019-2022', 'Tốt nghiệp', 'Đỗ Thị L', '0906666666', 1),
(7, 'Bùi Văn Khánh', NULL, '2010-01-19', 'Nam', 'Tày', 'Không', '56 Võ Văn Tần, TP. Hồ Chí Minh', '0978901234', 'khanh.bui@example.com', '2021-2024', 'Đang học', 'Bùi Văn N', '0907777777', 1),
(8, 'Ngô Thị Lan', NULL, '2011-06-05', 'Nữ', 'Kinh', 'Không', '78 Trần Hưng Đạo, TP. Hồ Chí Minh', '0989012345', 'lan.ngo@example.com', '2022-2025', 'Nghỉ học', 'Ngô Thị Q', '0908888888', 1),
(9, 'Vũ Văn Minh', NULL, '2009-08-17', 'Nam', 'Hoa', 'Không', '90 Pasteur, TP. Hồ Chí Minh', '0990123456', 'minh.vu@example.com', '2020-2023', 'Bảo lưu', 'Vũ Văn M', '0909999999', 1),
(10, 'Phan Thị Ngọc', NULL, '2011-12-25', 'Nữ', 'Khmer', 'Không', '321 Nguyễn Thị Minh Khai, TP. Hồ Chí Minh', '0901234567', 'ngoc.phan@example.com', '2021-2024', 'Đang học', 'Phan Thị P', '0910000001', 1),
(11, 'Nguyễn Văn A', NULL, '2007-05-12', 'Nam', 'Kinh', 'Không', '123 Lê Lợi, TP.HCM', '0901234567', 'nva@example.com', '2022-2025', 'Đang học', 'Nguyễn Văn B', '0910000002', 1),
(12, 'Trần Thị B', NULL, '2008-03-22', 'Nữ', 'Kinh', 'Phật giáo', '456 Nguyễn Huệ, TP.HCM', '0902345678', 'ttb@example.com', '2022-2025', 'Đang học', 'Trần Thị C', '0910000003', 1),
(13, 'Lê Văn C', NULL, '2007-11-30', 'Nam', 'Kinh', 'Thiên chúa', '789 Hai Bà Trưng, TP.HCM', '0903456789', 'lvc@example.com', '2022-2025', 'Đang học', 'Lê Văn D', '0910000004', 1),
(14, 'Phạm Thị D', NULL, '2008-07-15', 'Nữ', 'Kinh', 'Không', '321 Trần Hưng Đạo, TP.HCM', '0904567890', 'ptd@example.com', '2022-2025', 'Đang học', 'Phạm Thị E', '0910000005', 1),
(15, 'Hoàng Văn E', NULL, '2007-09-05', 'Nam', 'Kinh', 'Không', '654 Võ Văn Tần, TP.HCM', '0905678901', 'hve@example.com', '2022-2025', 'Đang học', 'Hoàng Văn F', '0910000006', 1),
(16, 'Hoàng Minh Quân', NULL, '2008-02-14', 'Nam', 'Kinh', 'Không', 'Phường 4, Q5, TP.HCM', '091234516', 'quan.hm16@example.com', '2023-2024', 'Đang học', 'Hoàng Hữu Tấn', '098765416', 1),
(17, 'Nguyễn Nhật Vy', NULL, '2008-09-30', 'Nữ', 'Kinh', 'Không', 'Phường Bình Trị Đông, Q.Bình Tân', '091234517', 'vy.nn17@example.com', '2023-2024', 'Đang học', 'Nguyễn Thành Công', '098765417', 1),
(18, 'Trần Gia Huy', NULL, '2008-11-20', 'Nam', 'Kinh', 'Không', 'Phường Tân Tạo, Q.Bình Tân', '091234518', 'huy.tg18@example.com', '2023-2024', 'Đang học', 'Trần Hữu Nghĩa', '098765418', 1),
(19, 'Phạm Khánh Linh', NULL, '2008-01-25', 'Nữ', 'Kinh', 'Không', 'Phường Linh Đông, TP.Thủ Đức', '091234519', 'linh.pk19@example.com', '2023-2024', 'Đang học', 'Phạm Quốc Hậu', '098765419', 1),
(20, 'Đỗ Nhật Minh', NULL, '2008-04-09', 'Nam', 'Kinh', 'Không', 'Phường Hiệp Bình Chánh, TP.Thủ Đức', '091234520', 'minh.dn20@example.com', '2023-2024', 'Đang học', 'Đỗ Văn Bửu', '098765420', 1),

(21, 'Võ Thảo My', NULL, '2008-07-11', 'Nữ', 'Kinh', 'Không', 'Phường Tân Hưng Thuận, Q12', '091234521', 'my.vt21@example.com', '2023-2024', 'Đang học', 'Võ Quốc Phong', '098765421', 1),
(22, 'Nguyễn Thành Đạt', NULL, '2008-05-18', 'Nam', 'Kinh', 'Không', 'Phường Đông Hưng Thuận, Q12', '091234522', 'dat.nt22@example.com', '2023-2024', 'Đang học', 'Nguyễn Hữu Trí', '098765422', 1),
(23, 'Lê Khánh An', NULL, '2008-10-12', 'Nữ', 'Kinh', 'Không', 'Phường Trường Thọ, TP.Thủ Đức', '091234523', 'an.lk23@example.com', '2023-2024', 'Đang học', 'Lê Văn Hòa', '098765423', 1),
(24, 'Bùi Gia Phúc', NULL, '2008-03-15', 'Nam', 'Kinh', 'Không', 'Phường 10, Q.Gò Vấp', '091234524', 'phuc.bg24@example.com', '2023-2024', 'Đang học', 'Bùi Quang Trí', '098765424', 1),
(25, 'Huỳnh Ngọc Mai', NULL, '2008-08-02', 'Nữ', 'Kinh', 'Không', 'Phường 9, Q.Gò Vấp', '091234525', 'mai.hn25@example.com', '2023-2024', 'Đang học', 'Huỳnh Thanh Bình', '098765425', 1),

(26, 'Nguyễn Anh Khoa', NULL, '2008-12-02', 'Nam', 'Kinh', 'Không', 'Phường 8, Q.Phú Nhuận', '091234526', 'khoa.na26@example.com', '2023-2024', 'Đang học', 'Nguyễn Hoàng Phúc', '098765426', 1),
(27, 'Trịnh Minh Châu', NULL, '2008-06-27', 'Nữ', 'Kinh', 'Không', 'Phường 7, Q.Phú Nhuận', '091234527', 'chau.tm27@example.com', '2023-2024', 'Đang học', 'Trịnh Đình Tài', '098765427', 1),
(28, 'Phan Quốc Bảo', NULL, '2008-09-17', 'Nam', 'Kinh', 'Không', 'Phường 15, Q.Tân Bình', '091234528', 'bao.pq28@example.com', '2023-2024', 'Đang học', 'Phan Công Minh', '098765428', 1),
(29, 'Đặng Ngọc Hân', NULL, '2008-11-08', 'Nữ', 'Kinh', 'Không', 'Phường 13, Q.Tân Bình', '091234529', 'han.dn29@example.com', '2023-2024', 'Đang học', 'Đặng Văn Thuận', '098765429', 1),
(30, 'Lý Minh Hoàng', NULL, '2008-02-19', 'Nam', 'Kinh', 'Không', 'Phường 5, Q3', '091234530', 'hoang.lm30@example.com', '2023-2024', 'Đang học', 'Lý Hữu Khánh', '098765430', 1),

(31, 'Hồ Thái Nghi', NULL, '2008-04-29', 'Nữ', 'Kinh', 'Không', 'Phường 3, Q10', '091234531', 'nghi.ht31@example.com', '2023-2024', 'Đang học', 'Hồ Văn Duy', '098765431', 1),
(32, 'Trương Gia Bảo', NULL, '2008-07-06', 'Nam', 'Kinh', 'Không', 'Phường 1, Q11', '091234532', 'bao.tg32@example.com', '2023-2024', 'Đang học', 'Trương Minh Quang', '098765432', 1),
(33, 'Ngô Nhật Minh', NULL, '2008-03-23', 'Nam', 'Kinh', 'Không', 'Phường 15, Q11', '091234533', 'minh.nn33@example.com', '2023-2024', 'Đang học', 'Ngô Văn Kiệt', '098765433', 1),
(34, 'Tạ Khánh Vy', NULL, '2008-01-18', 'Nữ', 'Kinh', 'Không', 'Phường 2, Q.Tân Phú', '091234534', 'vy.tk34@example.com', '2023-2024', 'Đang học', 'Tạ Quốc Vinh', '098765434', 1),
(35, 'Vương Tuấn Kiệt', NULL, '2008-05-09', 'Nam', 'Kinh', 'Không', 'Phường 9, Q.Tân Phú', '091234535', 'kiet.vt35@example.com', '2023-2024', 'Đang học', 'Vương Hoàng Lộc', '098765435', 1),

(36, 'Phùng Hoài Phương', NULL, '2008-09-05', 'Nữ', 'Kinh', 'Không', 'Phường Tân Sơn Nhì, Q.Tân Phú', '091234536', 'phuong.ph36@example.com', '2023-2024', 'Đang học', 'Phùng Thành Công', '098765436', 1),
(37, 'Đoàn Minh Trí', NULL, '2008-12-21', 'Nam', 'Kinh', 'Không', 'Phường Sơn Kỳ, Q.Tân Phú', '091234537', 'tri.dm37@example.com', '2023-2024', 'Đang học', 'Đoàn Quốc Bình', '098765437', 1),
(38, 'La Trúc Chi', NULL, '2008-08-16', 'Nữ', 'Kinh', 'Không', 'Phường Tây Thạnh, Q.Tân Phú', '091234538', 'chi.lt38@example.com', '2023-2024', 'Đang học', 'La Khánh Duy', '098765438', 1),
(39, 'Tôn Nhật Vũ', NULL, '2008-03-12', 'Nam', 'Kinh', 'Không', 'Phường 8, Q.Tân Bình', '091234539', 'vu.tn39@example.com', '2023-2024', 'Đang học', 'Tôn Thành Phú', '098765439', 1),
(40, 'Hứa Uyển Nhi', NULL, '2008-10-28', 'Nữ', 'Kinh', 'Không', 'Phường 14, Q10', '091234540', 'nhi.hu40@example.com', '2023-2024', 'Đang học', 'Hứa Chí Tín', '098765440', 1),

(41, 'Tăng Kiến Quốc', NULL, '2008-06-24', 'Nam', 'Kinh', 'Không', 'Phường 6, Q5', '091234541', 'quoc.tk41@example.com', '2023-2024', 'Đang học', 'Tăng Văn Hậu', '098765441', 1),
(42, 'Lâm Tuệ Nhi', NULL, '2008-02-06', 'Nữ', 'Kinh', 'Không', 'Phường 13, Q5', '091234542', 'nhi.lt42@example.com', '2023-2024', 'Đang học', 'Lâm Ngọc Hiếu', '098765442', 1),
(43, 'Diệp Quốc Hưng', NULL, '2008-11-01', 'Nam', 'Kinh', 'Không', 'Phường 4, Q10', '091234543', 'hung.dq43@example.com', '2023-2024', 'Đang học', 'Diệp Hữu Lộc', '098765443', 1),
(44, 'Sơn Hạ Vy', NULL, '2008-05-14', 'Nữ', 'Kinh', 'Không', 'Phường 3, Q3', '091234544', 'vy.sh44@example.com', '2023-2024', 'Đang học', 'Sơn Văn Minh', '098765444', 1),
(45, 'Trần Hoài Nam', NULL, '2008-09-09', 'Nam', 'Kinh', 'Không', 'Phường 4, Q8', '091234545', 'nam.th45@example.com', '2023-2024', 'Đang học', 'Trần Nhật Hào', '098765445', 1),

(46, 'Đặng Thanh Yên', NULL, '2008-04-11', 'Nữ', 'Kinh', 'Không', 'Phường 1, Q8', '091234546', 'yen.dt46@example.com', '2023-2024', 'Đang học', 'Đặng Minh Phú', '098765446', 1),
(47, 'Phạm Quốc Thiên', NULL, '2008-07-19', 'Nam', 'Kinh', 'Không', 'Phường 16, Q8', '091234547', 'thien.pq47@example.com', '2023-2024', 'Đang học', 'Phạm Công Hậu', '098765447', 1),
(48, 'Nguyễn Ngọc Hạ Vy', NULL, '2008-01-31', 'Nữ', 'Kinh', 'Không', 'Phường 5, Q4', '091234548', 'vy.nh48@example.com', '2023-2024', 'Đang học', 'Nguyễn Hoài Trung', '098765448', 1),
(49, 'Bạch Minh Khôi', NULL, '2008-03-08', 'Nam', 'Kinh', 'Không', 'Phường 6, Q4', '091234549', 'khoi.bm49@example.com', '2023-2024', 'Đang học', 'Bạch Công Lâm', '098765449', 1),
(50, 'Thái Ngọc Châu', NULL, '2008-10-03', 'Nữ', 'Kinh', 'Không', 'Phường 8, Q3', '091234550', 'chau.tn50@example.com', '2023-2024', 'Đang học', 'Thái Thanh Phong', '098765450', 1),
(51, 'Đinh Hữu Khải', NULL, '2011-05-19', 'Nam', 'Kinh', 'Không', 'Hải Châu, Đà Nẵng', '0912456789', 'khaidinh51@example.com', '2024-2025', 'Đang học', 'Đinh Hữu Long', '0912345001', 1),
(52, 'Nguyễn Hoài Phương', NULL, '2012-07-22', 'Nữ', 'Kinh', 'Không', 'Cẩm Lệ, Đà Nẵng', '0912451111', 'phuongnguyen52@example.com', '2024-2025', 'Đang học', 'Nguyễn Văn Dương', '0912345002', 1),
(53, 'Phạm Thanh Hòa', NULL, '2011-09-10', 'Nam', 'Kinh', 'Không', 'Liên Chiểu, Đà Nẵng', '0912452222', 'hoapham53@example.com', '2024-2025', 'Đang học', 'Phạm Minh Tâm', '0912345003', 1),
(54, 'Trần Mỹ An', NULL, '2012-11-08', 'Nữ', 'Kinh', 'Không', 'Thanh Khê, Đà Nẵng', '0912453333', 'antran54@example.com', '2024-2025', 'Đang học', 'Trần Đức Hòa', '0912345004', 1),
(55, 'Lê Quốc Hưng', NULL, '2011-03-14', 'Nam', 'Kinh', 'Không', 'Hòa Vang, Đà Nẵng', '0912454444', 'hungle55@example.com', '2024-2025', 'Đang học', 'Lê Quốc Thái', '0912345005', 1),
(56, 'Huỳnh Nhật Ánh', NULL, '2012-04-19', 'Nữ', 'Kinh', 'Không', 'Sơn Trà, Đà Nẵng', '0912455555', 'anhhuynh56@example.com', '2024-2025', 'Đang học', 'Huỳnh Đức Anh', '0912345006', 1),
(57, 'Bùi Thanh Lộc', NULL, '2011-06-25', 'Nam', 'Kinh', 'Không', 'Hải Châu, Đà Nẵng', '0912456666', 'locbui57@example.com', '2024-2025', 'Đang học', 'Bùi Văn Tường', '0912345007', 1),
(58, 'Võ Ngọc Duyên', NULL, '2012-01-29', 'Nữ', 'Kinh', 'Không', 'Cẩm Lệ, Đà Nẵng', '0912457777', 'duyenvo58@example.com', '2024-2025', 'Đang học', 'Võ Văn Bình', '0912345008', 1),
(59, 'Đặng Gia Huy', NULL, '2011-10-30', 'Nam', 'Kinh', 'Không', 'Liên Chiểu, Đà Nẵng', '0912458888', 'huydang59@example.com', '2024-2025', 'Đang học', 'Đặng Trọng Lực', '0912345009', 1),
(60, 'Ngô Kim Yến', NULL, '2012-02-05', 'Nữ', 'Kinh', 'Không', 'Thanh Khê, Đà Nẵng', '0912459999', 'yenngo60@example.com', '2024-2025', 'Đang học', 'Ngô Quốc Hào', '0912345010', 1),

(61, 'Lý Minh Tâm', NULL, '2011-07-07', 'Nam', 'Kinh', 'Không', 'Hòa Vang, Đà Nẵng', '0912460001', 'tamly61@example.com', '2024-2025', 'Đang học', 'Lý Thanh Huy', '0912345011', 1),
(62, 'Hồng Mỹ Linh', NULL, '2012-08-14', 'Nữ', 'Kinh', 'Không', 'Sơn Trà, Đà Nẵng', '0912460002', 'linhhong62@example.com', '2024-2025', 'Đang học', 'Hồng Văn Trí', '0912345012', 1),
(63, 'Trịnh Quốc Bảo', NULL, '2011-12-01', 'Nam', 'Kinh', 'Không', 'Hải Châu, Đà Nẵng', '0912460003', 'baotrinh63@example.com', '2024-2025', 'Đang học', 'Trịnh Văn Hòa', '0912345013', 1),
(64, 'Đoàn Thúy Vy', NULL, '2012-05-12', 'Nữ', 'Kinh', 'Không', 'Cẩm Lệ, Đà Nẵng', '0912460004', 'vydoan64@example.com', '2024-2025', 'Đang học', 'Đoàn Minh Lộc', '0912345014', 1),
(65, 'Tôn Nhật Minh', NULL, '2011-09-18', 'Nam', 'Kinh', 'Không', 'Liên Chiểu, Đà Nẵng', '0912460005', 'minhton65@example.com', '2024-2025', 'Đang học', 'Tôn Đức Cảnh', '0912345015', 1),
(66, 'La Trúc Quỳnh', NULL, '2012-03-23', 'Nữ', 'Kinh', 'Không', 'Thanh Khê, Đà Nẵng', '0912460006', 'quynhla66@example.com', '2024-2025', 'Đang học', 'La Minh Đạt', '0912345016', 1),
(67, 'Triệu Trung Kiên', NULL, '2011-04-09', 'Nam', 'Kinh', 'Không', 'Hòa Vang, Đà Nẵng', '0912460007', 'kientrieu67@example.com', '2024-2025', 'Đang học', 'Triệu Hoàng Phúc', '0912345017', 1),
(68, 'Cao Minh Châu', NULL, '2012-10-10', 'Nữ', 'Kinh', 'Không', 'Sơn Trà, Đà Nẵng', '0912460008', 'chaucao68@example.com', '2024-2025', 'Đang học', 'Cao Văn Sỹ', '0912345018', 1),
(69, 'Đinh Hồng Quân', NULL, '2011-11-21', 'Nam', 'Kinh', 'Không', 'Hải Châu, Đà Nẵng', '0912460009', 'quandinh69@example.com', '2024-2025', 'Đang học', 'Đinh Đức Trọng', '0912345019', 1),
(70, 'Mai Thúy Hằng', NULL, '2012-06-17', 'Nữ', 'Kinh', 'Không', 'Cẩm Lệ, Đà Nẵng', '0912460010', 'hangmai70@example.com', '2024-2025', 'Đang học', 'Mai Thanh Tùng', '0912345020', 1);

-- --------------------------------------------------------

--
-- Table structure for table `student_exams`
--

CREATE TABLE `student_exams` (
    `examinee_id` int(11),
    `student_id` int(11) NOT NULL,          -- Học sinh nào
    `exam_id` int(11) NOT NULL              -- Thuộc phòng thi nào
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `student_exams`
--

INSERT INTO `student_exams` (`examinee_id`, `student_id`, `exam_id`) VALUES
(1, 1, 1),
(2, 2, 1),
(3, 3, 2),
(4, 4, 3),
(5, 5, 2),
(6, 6, 3),
(7, 7, 3),
(8, 8, 5),
(9, 9, 4),
(10, 10, 5),
(11, 11, 5),
(12, 12, 2),
(13, 13, 1),
(14, 14, 3),
(15, 15, 1),
(16, 16, 4),
(17, 17, 5),
(18, 18, 2),
(19, 19, 3),
(20, 20, 4);

-- --------------------------------------------------------

--
-- Table structure for table `subjects`
--

CREATE TABLE `subjects` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `subjects`
--

INSERT INTO `subjects` (`id`, `name`, `description`, `status`) VALUES
(1, 'Toán học', 'Môn học về các con số, phép tính, hình học và tư duy logic.', 1),
(2, 'Ngữ văn', 'Môn học về tiếng Việt, văn học, kỹ năng đọc hiểu và viết luận.', 1),
(3, 'Tiếng Anh', 'Môn học về ngôn ngữ tiếng Anh, bao gồm từ vựng, ngữ pháp và giao tiếp.', 1),
(4, 'Vật lý', 'Môn học nghiên cứu các hiện tượng tự nhiên như chuyển động, lực, năng lượng.', 1),
(5, 'Hóa học', 'Môn học về cấu tạo, tính chất và sự biến đổi của chất.', 1),
(6, 'Sinh học', 'Môn học về cơ thể sống, sinh vật và môi trường sống.', 1),
(7, 'Lịch sử', 'Môn học về các sự kiện, nhân vật và quá trình phát triển của xã hội loài người.', 1),
(8, 'Địa lý', 'Môn học về vị trí địa lý, khí hậu, dân cư và các vùng lãnh thổ.', 1),
(9, 'Tin học', 'Môn học về máy tính, phần mềm, lập trình và ứng dụng công nghệ thông tin.', 1),
(10, 'Giáo dục công dân', 'Môn học về quyền và nghĩa vụ công dân, đạo đức và pháp luật.', 1);

-- --------------------------------------------------------

--
-- Table structure for table `subject_term_avg`
--

CREATE TABLE `subject_term_avg` (
  `assign_class_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `subject_id` int(11) NOT NULL,
  `score` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `subject_term_avg`
--

INSERT INTO `subject_term_avg` (`assign_class_id`, `student_id`, `subject_id`, `score`) VALUES
(12, 2, 1, 8.07),
(12, 2, 2, 8.75),
(12, 3, 1, 8.14),
(12, 3, 2, 8.18),
(12, 4, 1, 7.86),
(12, 4, 2, 8.64),
(12, 5, 1, 8.14),
(12, 5, 2, 8.38),
(12, 6, 1, 8.36),
(12, 6, 2, 8.61),
(12, 7, 1, 7.36),
(12, 7, 2, 7.50);

-- --------------------------------------------------------

--
-- Table structure for table `teachers`
--

CREATE TABLE `teachers` (
  `id` int(11) NOT NULL,
  `fullname` varchar(100) NOT NULL,
  `avatar` varchar(255) DEFAULT NULL,
  `birthday` date DEFAULT NULL,
  `gender` enum('Nam','Nữ') DEFAULT NULL,
  `address` varchar(200) DEFAULT NULL,
  `phone` varchar(10) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1,
  `user_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `teachers`
--

INSERT INTO `teachers`
(`id`, `fullname`, `avatar`, `birthday`, `gender`, `address`, `phone`, `email`, `status`, `user_id`)
VALUES
(1, 'Nguyễn Văn Hùng', NULL, '1980-03-15', 'Nam', '123 Lê Lợi, TP.HCM', '0901234567', 'nvhung@example.com', 1, 3),
(2, 'Trần Thị Mai', NULL, '1982-07-22', 'Nữ', '456 Nguyễn Huệ, TP.HCM', '0902345678', 'ttmai@example.com', 1, 4),
(3, 'Lê Văn Phúc', NULL, '1979-11-30', 'Nam', '789 Hai Bà Trưng, TP.HCM', '0903456789', 'lvphuc@example.com', 1, 5),
(4, 'Phạm Thị Hương', NULL, '1985-05-10', 'Nữ', '321 Trần Hưng Đạo, TP.HCM', '0904567890', 'pthuong@example.com', 1, 6),
(5, 'Hoàng Văn Tâm', NULL, '1981-09-05', 'Nam', '654 Võ Văn Tần, TP.HCM', '0905678901', 'hvtam@example.com', 1, 7),
(6, 'Đặng Thị Lan', NULL, '1983-01-18', 'Nữ', '987 Nguyễn Thị Minh Khai, TP.HCM', '0906789012', 'dtlan@example.com', 1, 8),
(7, 'Vũ Văn Quang', NULL, '1978-06-25', 'Nam', '159 Cách Mạng Tháng 8, TP.HCM', '0907890123', 'vvquang@example.com', 1, 9),
(8, 'Bùi Thị Ngọc', NULL, '1984-10-10', 'Nữ', '753 Điện Biên Phủ, TP.HCM', '0908901234', 'btngoc@example.com', 1, 10),
(9, 'Ngô Văn Sơn', NULL, '1980-12-01', 'Nam', '852 Nguyễn Đình Chiểu, TP.HCM', '0909012345', 'nvson@example.com', 1, 11),
(10, 'Dương Thị Thu', NULL, '1986-04-09', 'Nữ', '951 Lý Tự Trọng, TP.HCM', '0910123456', 'dtthu@example.com', 1, 12);
-- --------------------------------------------------------

--
-- Table structure for table `terms`
--

CREATE TABLE `terms` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `year` int(11) DEFAULT NULL,
  `learnyear` varchar(50) DEFAULT NULL,
  `start_date` date DEFAULT NULL,
  `end_date` date DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `terms`
--

INSERT INTO `terms` (`id`, `name`, `year`, `learnyear`, `start_date`, `end_date`, `status`) VALUES
(1, 'Học kỳ 1', 2021, '2021-2022', '2021-09-01', '2022-01-15', 1),
(2, 'Học kỳ 2', 2021, '2021-2022', '2022-02-01', '2022-06-15', 1),
(3, 'Học kỳ 1', 2022, '2022-2023', '2022-09-01', '2023-01-15', 1),
(4, 'Học kỳ 2', 2022, '2022-2023', '2023-02-01', '2023-06-15', 1),
(5, 'Học kỳ 1', 2023, '2023-2024', '2023-09-01', '2024-01-15', 1),
(6, 'Học kỳ 2', 2023, '2023-2024', '2024-02-01', '2024-06-15', 1),
(7, 'Học kỳ 1', 2024, '2024-2025', '2024-09-01', '2025-01-15', 1),
(8, 'Học kỳ 2', 2024, '2024-2025', '2025-02-01', '2025-06-15', 1),
(9, 'Học kỳ 1', 2025, '2025-2026', '2025-09-01', '2026-01-15', 1),
(10, 'Học kỳ 2', 2025, '2025-2026', '2026-02-01', '2026-06-15', 1);

-- --------------------------------------------------------

--
-- Table structure for table `term_gpa`
--

CREATE TABLE `term_gpa` (
  `assign_class_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `gpa` decimal(10,2) DEFAULT NULL,
  `conduct_level` enum('Giỏi','Khá','Trung bình','Yếu') DEFAULT NULL,
  `academic` enum('Giỏi','Khá','Trung bình','Yếu') DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `term_gpa`
--

INSERT INTO `term_gpa` (`assign_class_id`, `student_id`, `gpa`, `conduct_level`, `academic`) VALUES
(12, 2, 8.41, 'Giỏi', 'Giỏi'),
(12, 3, 8.16, 'Giỏi', 'Giỏi'),
(12, 4, 8.25, 'Giỏi', 'Giỏi'),
(12, 5, 8.26, 'Giỏi', 'Giỏi'),
(12, 6, 8.49, 'Giỏi', 'Giỏi'),
(12, 7, 7.43, 'Khá', 'Khá');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `role_id` int(11) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password` varchar(255) NOT NULL,
  `fullname` varchar(100) DEFAULT NULL,
  `avatar` varchar(255) DEFAULT NULL,
  `phone` varchar(15) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `address` varchar(200) DEFAULT NULL,
  `status` varchar(100) DEFAULT 'Hoạt động'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `role_id`, `username`, `password`, `fullname`, `avatar`, `phone`, `email`, `address`, `status`) VALUES
-- (1, 1, 'gv01', '123456', 'Trần Thị Giáo Viên', NULL, '0909234567', 'giaovien@example.com', '232 hưng phú phường chợ lớn Tp.HCM', 'Hoạt động'),
-- (2, 1, 'abc', 'abc123', 'Nguyễn Văn A', '', '0967548341', 'abc@gmail.com', '1', 'Hoạt động'),
(3, 1, 'nvhung', '123456', 'Nguyễn Văn Hùng', NULL, '0901234567', 'nvhung@example.com', '123 Lê Lợi, TP.HCM', 'Hoạt động'),
(4, 1, 'ttmai', '123456', 'Trần Thị Mai', NULL, '0902345678', 'ttmai@example.com', '456 Nguyễn Huệ, TP.HCM', 'Hoạt động'),
(5, 1, 'lvphuc', '123456', 'Lê Văn Phúc', NULL, '0903456789', 'lvphuc@example.com', '789 Hai Bà Trưng, TP.HCM', 'Hoạt động'),
(6, 1, 'pthuong', '123456', 'Phạm Thị Hương', NULL, '0904567890', 'pthuong@example.com', '321 Trần Hưng Đạo, TP.HCM', 'Hoạt động'),
(7, 1, 'hvtam', '123456', 'Hoàng Văn Tâm', NULL, '0905678901', 'hvtam@example.com', '654 Võ Văn Tần, TP.HCM', 'Hoạt động'),
(8, 1, 'dtlan', '123456', 'Đặng Thị Lan', NULL, '0906789012', 'dtlan@example.com', '987 Nguyễn Thị Minh Khai, TP.HCM', 'Hoạt động'),
(9, 1, 'vvquang', '123456', 'Vũ Văn Quang', NULL, '0907890123', 'vvquang@example.com', '159 Cách Mạng Tháng 8, TP.HCM', 'Hoạt động'),
(10, 1, 'btngoc', '123456', 'Bùi Thị Ngọc', NULL, '0908901234', 'btngoc@example.com', '753 Điện Biên Phủ, TP.HCM', 'Hoạt động'),
(11, 1, 'nvson', '123456', 'Ngô Văn Sơn', NULL, '0909012345', 'nvson@example.com', '852 Nguyễn Đình Chiểu, TP.HCM', 'Hoạt động'),
(12, 1, 'dtthu', '123456', 'Dương Thị Thu', NULL, '0910123456', 'dtthu@example.com', '951 Lý Tự Trọng, TP.HCM', 'Hoạt động'),
(13, 2, 'qtvien', '123456', 'Nguyễn Văn Quản Trị', NULL, '0000000001', 'qtvien@example.com', '001 hưng phú phường chợ lớn Tp.HCM', 'Hoạt động'),
(14, 3, 'qldaotao', '123456', 'Trần Thị Đào Tạo', NULL, '0000000002', 'qldaotao@example.com', '002 hưng phú phường chợ lớn Tp.HCM', 'Hoạt động'),
(15, 4, 'qltaichinh', '123456', 'Lê Tài chính', NULL, '0000000003', 'qltaichinh@example.com', '003 hưng phú phường chợ lớn Tp.HCM', 'Hoạt động'),
(16, 5, 'qlthongtin', '123456', 'Nguyễn Thông Tin', NULL, '0000000004', 'qlthongtin@example.com', '004 hưng phú phường chợ lớn Tp.HCM', 'Hoạt động'),
(17, 6, 'qlnguoidung', '123456', 'Võ Người dùng', NULL, '0000000005', 'qlnguoidung@example.com', '005 hưng phú phường chợ lớn Tp.HCM', 'Hoạt động');

-- --------------------------------------------------------

--
-- Table structure for table `violations`
--

CREATE TABLE `violations` (
  `id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `assign_class_id` int(11) NOT NULL,
  `rule_id` int(11) DEFAULT NULL,
  `date_create` date DEFAULT NULL,
  `description` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `violations`
--

INSERT INTO `violations` (`id`, `student_id`, `assign_class_id`, `rule_id`, `date_create`, `description`) VALUES
(1, 1, 11, 1, '2025-09-15', 'Đi học trễ 30 phút không có lý do.'),
(2, 2, 11, 2, '2025-09-16', 'Không mặc đồng phục theo quy định.'),
(3, 3, 12, 3, '2025-09-17', 'Xả rác trong lớp học.'),
(4, 4, 12, 4, '2025-09-18', 'Sử dụng điện thoại trong giờ học.'),
(5, 5, 13, 5, '2025-09-19', 'Nói chuyện hỗn với giáo viên.'),
(6, 6, 13, 6, '2025-09-20', 'Gây mất trật tự trong giờ kiểm tra.'),
(7, 7, 14, 7, '2025-09-21', 'Không tham gia buổi sinh hoạt lớp.'),
(8, 8, 14, 8, '2025-09-22', 'Mang thuốc lá vào trường.'),
(9, 9, 15, 9, '2025-09-23', 'Xô xát với bạn cùng lớp sau giờ học.'),
(10, 10, 15, 10, '2025-09-24', 'Làm hỏng thiết bị máy chiếu của lớp.');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `assign_classes`
--
ALTER TABLE `assign_classes`
  ADD PRIMARY KEY (`id`),
  ADD KEY `class_id` (`class_id`),
  ADD KEY `head_teacher_id` (`head_teacher_id`),
  ADD KEY `term_id` (`term_id`);

--
-- Indexes for table `assign_class_students`
--
ALTER TABLE `assign_class_students`
  ADD PRIMARY KEY (`assign_class_id`,`student_id`),
  ADD KEY `student_id` (`student_id`);

--
-- Indexes for table `assign_class_teachers`
--
ALTER TABLE `assign_class_teachers`
  ADD PRIMARY KEY (`assign_class_id`,`teacher_id`,`subject_id`),
  ADD KEY `teacher_id` (`teacher_id`),
  ADD KEY `subject_id` (`subject_id`);

--
-- Indexes for table `classes`
--
ALTER TABLE `classes`
  ADD PRIMARY KEY (`id`),
  ADD KEY `class_type_id` (`class_type_id`);

--
-- Indexes for table `class_types`
--
ALTER TABLE `class_types`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `departments`
--
ALTER TABLE `departments`
  ADD PRIMARY KEY (`id`),
  ADD KEY `subject_id` (`subject_id`);

--
-- Indexes for table `department_details`
--
ALTER TABLE `department_details`
  ADD PRIMARY KEY (`department_id`,`teacher_id`),
  ADD KEY `teacher_id` (`teacher_id`);

--
-- Indexes for table `exams`
--
ALTER TABLE `exams`
  ADD PRIMARY KEY (`id`,`exam_detail_id`),
  ADD KEY `exam_detail_id` (`exam_detail_id`);

--
-- Indexes for table `rooms`
--
ALTER TABLE `rooms`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `exam_details`
--
ALTER TABLE `exam_details`
  ADD PRIMARY KEY (`id`,`subject_id`,`term_id`),
  ADD KEY `subject_id` (`subject_id`),
  ADD KEY `term_id` (`term_id`);

--
-- Indexes for table `exam_types`
--
ALTER TABLE `exam_types`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `functions`
--
ALTER TABLE `functions`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `roles`
--
ALTER TABLE `roles`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `role_details`
--
ALTER TABLE `role_details`
  ADD PRIMARY KEY (`role_id`,`function_id`,`action`),
  ADD KEY `function_id` (`function_id`);

--
-- Indexes for table `rules`
--
ALTER TABLE `rules`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `score_details`
--
ALTER TABLE `score_details`
  ADD PRIMARY KEY (`assign_class_id`,`student_id`,`subject_id`,`exam_type_id`,`attempt`),
  ADD KEY `student_id` (`student_id`),
  ADD KEY `subject_id` (`subject_id`),
  ADD KEY `exam_type_id` (`exam_type_id`);

--
-- Indexes for table `students`
--
ALTER TABLE `students`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `student_exams`
--
ALTER TABLE `student_exams`
  ADD PRIMARY KEY (`examinee_id`,`student_id`,`exam_id`),
  ADD KEY `student_id` (`student_id`),
  ADD KEY `exam_id` (`exam_id`);

--
-- Indexes for table `subjects`
--
ALTER TABLE `subjects`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `subject_term_avg`
--
ALTER TABLE `subject_term_avg`
  ADD PRIMARY KEY (`assign_class_id`,`student_id`,`subject_id`),
  ADD KEY `student_id` (`student_id`),
  ADD KEY `subject_id` (`subject_id`);

--
-- Indexes for table `teachers`
--
ALTER TABLE `teachers`
  ADD PRIMARY KEY (`id`),
  ADD KEY `user_id` (`user_id`);

--
-- Indexes for table `terms`
--
ALTER TABLE `terms`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `term_gpa`
--
ALTER TABLE `term_gpa`
  ADD PRIMARY KEY (`assign_class_id`,`student_id`),
  ADD KEY `student_id` (`student_id`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `username` (`username`),
  ADD KEY `role_id` (`role_id`);

--
-- Indexes for table `violations`
--
ALTER TABLE `violations`
  ADD PRIMARY KEY (`id`),
  ADD KEY `student_id` (`student_id`),
  ADD KEY `assign_class_id` (`assign_class_id`),
  ADD KEY `rule_id` (`rule_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `assign_classes`
--
ALTER TABLE `assign_classes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT for table `classes`
--
ALTER TABLE `classes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `class_types`
--
ALTER TABLE `class_types`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `departments`
--
ALTER TABLE `departments`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `exams`
--
ALTER TABLE `exams`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `exam_details`
--
ALTER TABLE `exam_details`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `exam_types`
--
ALTER TABLE `exam_types`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `functions`
--
ALTER TABLE `functions`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `roles`
--
ALTER TABLE `roles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `rules`
--
ALTER TABLE `rules`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `students`
--
ALTER TABLE `students`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=51;

--
-- AUTO_INCREMENT for table `student_exams`
--
ALTER TABLE `student_exams`
  MODIFY `examinee_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT for table `subjects`
--
ALTER TABLE `subjects`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `teachers`
--
ALTER TABLE `teachers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `terms`
--
ALTER TABLE `terms`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `violations`
--
ALTER TABLE `violations`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `assign_classes`
--
ALTER TABLE `assign_classes`
  ADD CONSTRAINT `assign_classes_ibfk_1` FOREIGN KEY (`class_id`) REFERENCES `classes` (`id`),
  ADD CONSTRAINT `assign_classes_ibfk_2` FOREIGN KEY (`head_teacher_id`) REFERENCES `teachers` (`id`),
  ADD CONSTRAINT `assign_classes_ibfk_3` FOREIGN KEY (`term_id`) REFERENCES `terms` (`id`);

--
-- Constraints for table `assign_class_students`
--
ALTER TABLE `assign_class_students`
  ADD CONSTRAINT `assign_class_students_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `assign_class_students_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);

--
-- Constraints for table `assign_class_teachers`
--
ALTER TABLE `assign_class_teachers`
  ADD CONSTRAINT `assign_class_teachers_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `assign_class_teachers_ibfk_2` FOREIGN KEY (`teacher_id`) REFERENCES `teachers` (`id`),
  ADD CONSTRAINT `assign_class_teachers_ibfk_3` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`);

--
-- Constraints for table `classes`
--
ALTER TABLE `classes`
  ADD CONSTRAINT `classes_ibfk_1` FOREIGN KEY (`class_type_id`) REFERENCES `class_types` (`id`);

--
-- Constraints for table `departments`
--
ALTER TABLE `departments`
  ADD CONSTRAINT `departments_ibfk_1` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`);

--
-- Constraints for table `department_details`
--
ALTER TABLE `department_details`
  ADD CONSTRAINT `department_details_ibfk_1` FOREIGN KEY (`department_id`) REFERENCES `departments` (`id`),
  ADD CONSTRAINT `department_details_ibfk_2` FOREIGN KEY (`teacher_id`) REFERENCES `teachers` (`id`);

--
-- Constraints for table `exams`
--
ALTER TABLE `exams`
  ADD CONSTRAINT `exams_ibfk_1` FOREIGN KEY (`exam_detail_id`) REFERENCES `exam_details`(`id`),
  ADD CONSTRAINT `exams_ibfk_2` FOREIGN KEY (`supervisor_id`) REFERENCES `teachers`(`id`),
  ADD CONSTRAINT `exams_ibfk_3` FOREIGN KEY (`exam_room`) REFERENCES `rooms`(`id`);

--
-- Constraints for table `exam_details`
--
ALTER TABLE `exam_details`
  ADD CONSTRAINT `exam_details_ibfk_1` FOREIGN KEY (`subject_id`) REFERENCES `subjects`(`id`),
  ADD CONSTRAINT `exam_details_ibfk_2` FOREIGN KEY (`term_id`) REFERENCES `terms`(`id`),
  ADD CONSTRAINT `exam_details_ibfk_3` FOREIGN KEY (`exam_type_id`) REFERENCES `exam_types`(`id`);

--
-- Constraints for table `role_details`
--
ALTER TABLE `role_details`
  ADD CONSTRAINT `role_details_ibfk_1` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`),
  ADD CONSTRAINT `role_details_ibfk_2` FOREIGN KEY (`function_id`) REFERENCES `functions` (`id`);

--
-- Constraints for table `score_details`
--
ALTER TABLE `score_details`
  ADD CONSTRAINT `score_details_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `score_details_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`),
  ADD CONSTRAINT `score_details_ibfk_3` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`),
  ADD CONSTRAINT `score_details_ibfk_4` FOREIGN KEY (`exam_type_id`) REFERENCES `exam_types` (`id`);

--
-- Constraints for table `student_exams`
--
ALTER TABLE `student_exams`
  ADD CONSTRAINT `student_exams_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`),
  ADD CONSTRAINT `student_exams_ibfk_2` FOREIGN KEY (`exam_id`) REFERENCES `exams` (`id`);

--
-- Constraints for table `subject_term_avg`
--
ALTER TABLE `subject_term_avg`
  ADD CONSTRAINT `subject_term_avg_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `subject_term_avg_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`),
  ADD CONSTRAINT `subject_term_avg_ibfk_3` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`);

--
-- Constraints for table `term_gpa`
--
ALTER TABLE `term_gpa`
  ADD CONSTRAINT `term_gpa_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `term_gpa_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);

--
-- Constraints for table `users`
--
ALTER TABLE `users`
  ADD CONSTRAINT `users_ibfk_1` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`);

--
-- Constraints for table `violations`
--
ALTER TABLE `violations`
  ADD CONSTRAINT `violations_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`),
  ADD CONSTRAINT `violations_ibfk_2` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `violations_ibfk_3` FOREIGN KEY (`rule_id`) REFERENCES `rules` (`id`);

-- cập nhật trung bình môn
DELIMITER //

-- ============================================
-- Trigger 1: Cập nhật subject_term_avg khi INSERT score_details
-- ============================================
DROP TRIGGER IF EXISTS calc_subject_avg_after_insert//
CREATE TRIGGER calc_subject_avg_after_insert
AFTER INSERT ON score_details 
FOR EACH ROW
BEGIN
    REPLACE INTO subject_term_avg (assign_class_id, student_id, subject_id, score)
    SELECT
        assign_class_id,
        student_id,
        subject_id,
        SUM(score *
            CASE exam_type_id
                WHEN 1 THEN 1
                WHEN 2 THEN 1
                WHEN 3 THEN 2
                WHEN 4 THEN 3
            END)
        /
        SUM(
            CASE exam_type_id
                WHEN 1 THEN 1
                WHEN 2 THEN 1
                WHEN 3 THEN 2
                WHEN 4 THEN 3
            END) AS avg_score
    FROM score_details
    WHERE assign_class_id = NEW.assign_class_id
    AND student_id = NEW.student_id
    AND subject_id = NEW.subject_id
    GROUP BY assign_class_id, student_id, subject_id;
END //

-- ============================================
-- Trigger 2: Cập nhật subject_term_avg khi UPDATE score_details
-- ============================================
DROP TRIGGER IF EXISTS calc_subject_avg_after_update//
CREATE TRIGGER calc_subject_avg_after_update
AFTER UPDATE ON score_details 
FOR EACH ROW
BEGIN
    REPLACE INTO subject_term_avg (assign_class_id, student_id, subject_id, score)
    SELECT
        assign_class_id,
        student_id,
        subject_id,
        SUM(score *
            CASE exam_type_id
                WHEN 1 THEN 1
                WHEN 2 THEN 1
                WHEN 3 THEN 2
                WHEN 4 THEN 3
            END)
        /
        SUM(
            CASE exam_type_id
                WHEN 1 THEN 1
                WHEN 2 THEN 1
                WHEN 3 THEN 2
                WHEN 4 THEN 3
            END) AS avg_score
    FROM score_details
    WHERE assign_class_id = NEW.assign_class_id
    AND student_id = NEW.student_id
    AND subject_id = NEW.subject_id
    GROUP BY assign_class_id, student_id, subject_id;
END //

-- ============================================
-- Trigger 3: Cập nhật subject_term_avg khi DELETE score_details
-- ============================================
DROP TRIGGER IF EXISTS calc_subject_avg_after_delete//
CREATE TRIGGER calc_subject_avg_after_delete
AFTER DELETE ON score_details 
FOR EACH ROW
BEGIN
    DECLARE remaining_count INT;
    
    SELECT COUNT(*)
    INTO remaining_count
    FROM score_details
    WHERE assign_class_id = OLD.assign_class_id
    AND student_id = OLD.student_id
    AND subject_id = OLD.subject_id;
    
    IF remaining_count > 0 THEN
        REPLACE INTO subject_term_avg (assign_class_id, student_id, subject_id, score)
        SELECT
            assign_class_id,
            student_id,
            subject_id,
            SUM(score *
                CASE exam_type_id
                    WHEN 1 THEN 1
                    WHEN 2 THEN 1
                    WHEN 3 THEN 2
                    WHEN 4 THEN 3
                END)
            /
            SUM(
                CASE exam_type_id
                    WHEN 1 THEN 1
                    WHEN 2 THEN 1
                    WHEN 3 THEN 2
                    WHEN 4 THEN 3
                END) AS avg_score
        FROM score_details
        WHERE assign_class_id = OLD.assign_class_id
        AND student_id = OLD.student_id
        AND subject_id = OLD.subject_id
        GROUP BY assign_class_id, student_id, subject_id;
    ELSE
        DELETE FROM subject_term_avg
        WHERE assign_class_id = OLD.assign_class_id
        AND student_id = OLD.student_id
        AND subject_id = OLD.subject_id;
    END IF;
END //

-- ============================================
-- Trigger 4: Cập nhật term_gpa khi INSERT subject_term_avg
-- ============================================
DROP TRIGGER IF EXISTS trg_gpa_after_insert//
CREATE TRIGGER trg_gpa_after_insert
AFTER INSERT ON subject_term_avg
FOR EACH ROW
BEGIN
    DECLARE gpa_val FLOAT;
    DECLARE current_conduct VARCHAR(50);

    SELECT AVG(score)
    INTO gpa_val
    FROM subject_term_avg
    WHERE assign_class_id = NEW.assign_class_id
      AND student_id = NEW.student_id;

    SELECT conduct_level
    INTO current_conduct
    FROM term_gpa
    WHERE assign_class_id = NEW.assign_class_id
      AND student_id = NEW.student_id;

    REPLACE INTO term_gpa(assign_class_id, student_id, gpa, conduct_level, academic)
    VALUES(
        NEW.assign_class_id,
        NEW.student_id,
        gpa_val,
        current_conduct,
        CASE
            WHEN current_conduct IS NULL THEN NULL
            WHEN gpa_val < 5 OR current_conduct = 'Yếu' THEN 'Yếu' 
            WHEN gpa_val >= 8 AND current_conduct = 'Giỏi' THEN 'Giỏi' 
            WHEN gpa_val >= 8 AND current_conduct = 'Khá' THEN 'Khá' 
            WHEN gpa_val >= 6.5 AND current_conduct IN ('Khá', 'Giỏi') THEN 'Khá' 
            ELSE 'Trung bình'
        END
    );
END //

-- ============================================
-- Trigger 5: Cập nhật term_gpa khi UPDATE subject_term_avg
-- ============================================
DROP TRIGGER IF EXISTS trg_gpa_after_update//
CREATE TRIGGER trg_gpa_after_update
AFTER UPDATE ON subject_term_avg
FOR EACH ROW
BEGIN
    DECLARE gpa_val FLOAT;
    DECLARE current_conduct VARCHAR(50);

    SELECT AVG(score)
    INTO gpa_val
    FROM subject_term_avg
    WHERE assign_class_id = NEW.assign_class_id
      AND student_id = NEW.student_id;

    SELECT conduct_level
    INTO current_conduct
    FROM term_gpa
    WHERE assign_class_id = NEW.assign_class_id
      AND student_id = NEW.student_id;

    REPLACE INTO term_gpa(assign_class_id, student_id, gpa, conduct_level, academic)
    VALUES(
        NEW.assign_class_id,
        NEW.student_id,
        gpa_val,
        current_conduct,
        CASE
            WHEN current_conduct IS NULL THEN NULL
            WHEN gpa_val < 5 OR current_conduct = 'Yếu' THEN 'Yếu' 
            WHEN gpa_val >= 8 AND current_conduct = 'Giỏi' THEN 'Giỏi' 
            WHEN gpa_val >= 8 AND current_conduct = 'Khá' THEN 'Khá' 
            WHEN gpa_val >= 6.5 AND current_conduct IN ('Khá', 'Giỏi') THEN 'Khá' 
            ELSE 'Trung bình'
        END
    );
END //

-- ============================================
-- Trigger 6: Cập nhật term_gpa khi DELETE subject_term_avg
-- ============================================
DROP TRIGGER IF EXISTS trg_gpa_after_delete//
CREATE TRIGGER trg_gpa_after_delete
AFTER DELETE ON subject_term_avg
FOR EACH ROW
BEGIN
    DECLARE gpa_val FLOAT;
    DECLARE current_conduct VARCHAR(50);

    SELECT AVG(score)
    INTO gpa_val
    FROM subject_term_avg
    WHERE assign_class_id = OLD.assign_class_id
      AND student_id = OLD.student_id;

    IF gpa_val IS NOT NULL THEN
        SELECT conduct_level
        INTO current_conduct
        FROM term_gpa
        WHERE assign_class_id = OLD.assign_class_id
          AND student_id = OLD.student_id;

        REPLACE INTO term_gpa(assign_class_id, student_id, gpa, conduct_level, academic)
        VALUES(
            OLD.assign_class_id,
            OLD.student_id,
            gpa_val,
            current_conduct,
            CASE
                WHEN current_conduct IS NULL THEN NULL
                WHEN gpa_val < 5 OR current_conduct = 'Yếu' THEN 'Yếu' 
				WHEN gpa_val >= 8 AND current_conduct = 'Giỏi' THEN 'Giỏi' 
				WHEN gpa_val >= 8 AND current_conduct = 'Khá' THEN 'Khá' 
				WHEN gpa_val >= 6.5 AND current_conduct IN ('Khá', 'Giỏi') THEN 'Khá' 
				ELSE 'Trung bình'
            END
        );
    ELSE
        DELETE FROM term_gpa
        WHERE assign_class_id = OLD.assign_class_id
          AND student_id = OLD.student_id;
    END IF;
END //

DELIMITER ;

-- 1) Months: danh sách các tháng (dùng chung)
CREATE TABLE `months` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL COMMENT 'Ví dụ: Tháng 8, Tháng 9, ...',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


-- 2) Fee templates: định nghĩa loại phí
CREATE TABLE `fee_templates` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(200) NOT NULL COMMENT 'Tên phí: Học phí, Tiền xe, Bữa ăn, ...',
  `description` TEXT NULL,
  `fee_type` ENUM('BASE','EXTRA') NOT NULL DEFAULT 'BASE' COMMENT 'BASE = phí cơ bản, EXTRA = phí phát sinh/khác',
  `amount` DECIMAL(12,2) NOT NULL DEFAULT 0,
  `is_active` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_fee_type` (`fee_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `class_fee_months` (
  `id` INT NOT NULL AUTO_INCREMENT,


  `assign_class_id` INT NOT NULL,


  `fee_template_id` INT NOT NULL,

  `month_id` INT NOT NULL,

  `term` TINYINT(1) NOT NULL COMMENT '1 = HK1, 2 = HK2',

 
  `is_selected` TINYINT(1) NOT NULL DEFAULT 1,

  `amount` DECIMAL(12,2) NOT NULL DEFAULT 0,

  `start_date` DATE DEFAULT NULL,
  `end_date` DATE DEFAULT NULL,

  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

  PRIMARY KEY (`id`),

  KEY `idx_class_fee_months_assign` (`assign_class_id`),
  KEY `idx_class_fee_months_template` (`fee_template_id`),
  KEY `idx_class_fee_months_month` (`month_id`),

  CONSTRAINT `fk_cfm_assign_class` FOREIGN KEY (`assign_class_id`)
       REFERENCES `assign_classes` (`id`) ON DELETE CASCADE,

  CONSTRAINT `fk_cfm_fee_template` FOREIGN KEY (`fee_template_id`)
       REFERENCES `fee_templates` (`id`) ON DELETE CASCADE,

  CONSTRAINT `fk_cfm_month` FOREIGN KEY (`month_id`)
       REFERENCES `months` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- 12) TuitionPayment: các lần thanh toán (ghi log)
CREATE TABLE tuition_monthly (
    id INT AUTO_INCREMENT PRIMARY KEY,

    student_id INT NOT NULL,
    assign_class_id INT NOT NULL,
    month_id INT NOT NULL,

    total_amount DECIMAL(12,2) NOT NULL DEFAULT 0,

    is_paid TINYINT(1) NOT NULL DEFAULT 0, -- 0 = chưa đóng, 1 = đã đóng

    collected_by INT NULL, -- id người thu học phí (user)
    collected_at DATETIME NULL, -- ngày giờ thu học phí

    payment_method VARCHAR(50) NULL, -- hình thức thanh toán
    note TEXT NULL, -- ghi chú

    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT fk_tm_student
        FOREIGN KEY (student_id) REFERENCES students(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_tm_assign_class
        FOREIGN KEY (assign_class_id) REFERENCES assign_class_students(assign_class_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_tm_month
        FOREIGN KEY (month_id) REFERENCES months(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_tm_collected_by
        FOREIGN KEY (collected_by) REFERENCES users(id)
        ON DELETE SET NULL
);




INSERT INTO months (id, name) VALUES
(1, 'Tháng 1'),
(2, 'Tháng 2'),
(3, 'Tháng 3'),
(4, 'Tháng 4'),
(5, 'Tháng 5'),
(6, 'Tháng 6'),
(7, 'Tháng 7'),
(8, 'Tháng 8'),
(9, 'Tháng 9'),
(10, 'Tháng 10'),
(11, 'Tháng 11'),
(12, 'Tháng 12');

COMMIT;