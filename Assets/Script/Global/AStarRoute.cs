using System;
using System.Collections.Generic;

public class AStarRoute
{
	private int[,] map;  // 地图矩阵，0表示能通过，1表示不能通过
	private int map_w;    // 地图宽度
	private int map_h;    // 地图高度
	private int start_x;  // 起点坐标X
	private int start_y;  // 起点坐标Y
	private int goal_x;   // 终点坐标X
	private int goal_y;   // 终点坐标Y
	
	private Boolean[,]closeList;            // 关闭列表
	public  int[,,]openList;               // 打开列表
	private int openListLength;
	private  const int EXIST = 1;
	private  const int NOT_EXIST = 0;
	private  const int ISEXIST = 0;
	private  const int EXPENSE = 1;     // 自身的代价
	private  const int DISTANCE = 2;    // 距离的代价
	private  const int COST = 3;        // 消耗的总代价
	private  const int FATHER_DIR = 4;  // 父节点的方向
	
	public   const int DIR_NULL = 0;
	public   const int DIR_DOWN = 1;     // 方向：下
	public   const int DIR_UP = 2;       // 方向：上
	public   const int DIR_LEFT = 3;     // 方向：左
	public  const int DIR_RIGHT = 4;    // 方向：右
	public  const int DIR_UP_LEFT = 5;
	public  const int DIR_UP_RIGHT = 6;
	public  const int DIR_DOWN_LEFT = 7;
	public  const int DIR_DOWN_RIGHT = 8;
	private int astar_counter;                // 算法嵌套深度
	private Boolean isFound;                  // 是否找到路径
	
	public AStarRoute (int[,] mx, int sx, int sy, int gx, int gy)
	{
		start_x = sx;
		start_y = sy;
		goal_x = gx;
		goal_y = gy;
		map = mx;
		map_w = mx.Length;
		map_h = mx.GetLength (0);
		astar_counter = 5000;
		initCloseList ();
		initOpenList (goal_x, goal_y);
	}
	
	// 得到地图上这一点的消耗值
	private int getMapExpense (int x, int y, int dir)
	{
		if (dir < 5) {
			return 10;
		} else {
			return 14;
		}
	}
	
	// 得到距离的消耗值
	private int getDistance (int x, int y, int ex, int ey)
	{
		return 10 * (Math.Abs (x - ex) + Math.Abs (y - ey));
	}
	
	// 得到给定坐标格子此时的总消耗值
	private int getCost (int x, int y)
	{
		return openList [x, y, COST];
	}
	
	// 开始寻路
	public void searchPath ()
	{
		addOpenList (start_x, start_y);
		aStar (start_x, start_y);
	}
	
	// 寻路
	private void aStar (int x, int y)
	{
		// 控制算法深度
		for (int t = 0; t < astar_counter; t++) {
			if (((x == goal_x) && (y == goal_y))) {
				isFound = true;
				return;
			} else if ((openListLength == 0)) {
				isFound = false;
				return;
			}
			
			removeOpenList (x, y);
			addCloseList (x, y);
			
			// 该点周围能够行走的点
			addNewOpenList (x, y, x, y + 1, DIR_UP);
			addNewOpenList (x, y, x, y - 1, DIR_DOWN);
			addNewOpenList (x, y, x - 1, y, DIR_RIGHT);
			addNewOpenList (x, y, x + 1, y, DIR_LEFT);
			addNewOpenList (x, y, x + 1, y + 1, DIR_UP_LEFT);
			addNewOpenList (x, y, x - 1, y + 1, DIR_UP_RIGHT);
			addNewOpenList (x, y, x + 1, y - 1, DIR_DOWN_LEFT);
			addNewOpenList (x, y, x - 1, y - 1, DIR_DOWN_RIGHT);
			
			// 找到估值最小的点，进行下一轮算法
			int cost = 0x7fffffff;
			for (int i = 0; i < map_w; i++) {
				for (int j = 0; j < map_h; j++) {
					if (openList [i, j, ISEXIST] == EXIST) {
						if (cost > getCost (i, j)) {
							cost = getCost (i, j);
							x = i;
							y = j;
						}
					}
				}
			}
		}
		// 算法超深
		isFound = false;
		return;
	}
	
	// 添加一个新的节点
	private void addNewOpenList (int x, int y, int newX, int newY, int dir)
	{
		if (isCanPass (newX, newY)) {
			if (openList [newX, newY, ISEXIST] == EXIST) {
				if (openList [x, y, EXPENSE] + getMapExpense (newX, newY, dir) < 
					openList [newX, newY, EXPENSE]) {
					setFatherDir (newX, newY, dir);
					setCost (newX, newY, x, y, dir);
				}
			} else {
				addOpenList (newX, newY);
				setFatherDir (newX, newY, dir);
				setCost (newX, newY, x, y, dir);
			}
		}
	}
	
	// 设置消耗值
	private void setCost (int x, int y, int ex, int ey, int dir)
	{
		openList [x, y, EXPENSE] = openList [ex, ey, EXPENSE] + getMapExpense (x, y, dir);
		openList [x, y, DISTANCE] = getDistance (x, y, ex, ey);
		openList [x, y, COST] = openList [x, y, EXPENSE] + openList [x, y, DISTANCE];
	}
	
	// 设置父节点方向
	private void setFatherDir (int x, int y, int dir)
	{
		openList [x, y, FATHER_DIR] = dir;
	}
	
	// 判断一个点是否可以通过
	private Boolean isCanPass (int x, int y)
	{
		// 超出边界
		if (x < 0 || x >= map_w || y < 0 || y >= map_h) {
			return false;
		}
		// 地图不通
		if (map [x, y] != 0) {
			return false;
		}
		// 在关闭列表中
		if (isInCloseList (x, y)) {
			return false;
		}
		return true;
	}
	
	// 移除打开列表的一个元素
	private void removeOpenList (int x, int y)
	{
		if (openList [x, y, ISEXIST] == EXIST) {
			openList [x, y, ISEXIST] = NOT_EXIST;
			openListLength--;
		}
	}
	
	// 判断一点是否在关闭列表中
	private Boolean isInCloseList (int x, int y)
	{
		return closeList [x, y];
	}
	
	// 添加关闭列表
	private void addCloseList (int x, int y)
	{
		closeList [x, y] = true;
	}
	
	// 添加打开列表
	private void addOpenList (int x, int y)
	{
		if (openList [x, y, ISEXIST] == NOT_EXIST) {
			openList [x, y, ISEXIST] = EXIST;
			openListLength++;
		}
	}
	
	// 初始化关闭列表
	private void initCloseList ()
	{
		closeList = new Boolean[map_w, map_h];
		for (int i = 0; i < map_w; i++) {
			for (int j = 0; j < map_h; j++) {
				closeList [i, j] = false;
			}
		}
	}
	
	// 初始化打开列表
	private void initOpenList (int ex, int ey)
	{
		openList = new int[map_w, map_h, 5];
		for (int i = 0; i < map_w; i++) {
			for (int j = 0; j < map_h; j++) {
				openList [i, j, ISEXIST] = NOT_EXIST;
				openList [i, j, EXPENSE] = getMapExpense (i, j, DIR_NULL);
				openList [i, j, DISTANCE] = getDistance (i, j, ex, ey);
				openList [i, j, COST] = openList [i, j, EXPENSE] + openList [i, j, DISTANCE];
				openList [i, j, FATHER_DIR] = DIR_NULL;
			}
		}
		openListLength = 0;
	}
	
	// 获得寻路结果
	public route_pt[] getResult ()
	{
		route_pt[] result;
		List<route_pt> route;
		searchPath ();
		if (! isFound) {
			return null;
		}
		route = new List<route_pt> ();
		// openList是从目标点向起始点倒推的。
		int iX = goal_x;
		int iY = goal_y;
		while ((iX != start_x || iY != start_y)) {
			route.Add (new route_pt (iX, iY));
			switch (openList [iX, iY, FATHER_DIR]) {
			case DIR_DOWN:          
				iY++;            
				break;
			case DIR_UP:            
				iY--;            
				break;
			case DIR_LEFT:          
				iX--;            
				break;
			case DIR_RIGHT:         
				iX++;            
				break;
			case DIR_UP_LEFT:       
				iX--;   
				iY--;    
				break;
			case DIR_UP_RIGHT:      
				iX++;   
				iY--;    
				break;
			case DIR_DOWN_LEFT:     
				iX--;   
				iY++;    
				break;
			case DIR_DOWN_RIGHT:    
				iX++;   
				iY++;    
				break;
			}
		}
		int size = route.Count;
		result = new route_pt[size];
		for (int i = 0; i < size; i++) {
			result [i] = new route_pt ((route_pt)route [i]);
		}
		return result;
	}
}

public class route_pt
{


	public route_pt (route_pt r)
	{
		this.x = r.x;
		this.y = r.y;
	}

	public route_pt (int a, int b)
	{
		this.x = a;
		this.y = b;
	}
	public int x;
	public int y;
}
